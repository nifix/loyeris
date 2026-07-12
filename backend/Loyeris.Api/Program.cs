using Loyeris.Api.Endpoints;
using Loyeris.Api.Configuration;
using Loyeris.Api.Health;
using Loyeris.Api.Operations;
using Loyeris.Api.Security;
using Loyeris.IdentityAccess.App;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Infrastructure;
using Loyeris.Leasing.App;
using Loyeris.Leasing.Infrastructure;
using Loyeris.Messaging.App;
using Loyeris.Messaging.Infrastructure;
using Loyeris.Portfolio.App;
using Loyeris.Portfolio.Infrastructure;
using Loyeris.RentCollection.App;
using Loyeris.RentCollection.Infrastructure;
using Loyeris.TaxPreparation.App;
using Loyeris.TaxPreparation.Infrastructure;
using Loyeris.Shared.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), ["live"])
    .AddCheck<PostgreSqlHealthCheck>("postgresql", tags: ["ready"]);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddOptions<ApplicationUrlOptions>()
    .Bind(builder.Configuration.GetSection(ApplicationUrlOptions.SectionName))
    .Validate(ConfigurationValidation.HasValidFrontendBaseUrl,
        "ApplicationUrls:FrontendBaseUrl must be an absolute HTTP or HTTPS URL.")
    .ValidateOnStart();

builder.Services.AddOptions<SmtpOptions>()
    .Bind(builder.Configuration.GetSection(SmtpOptions.SectionName))
    .Validate(ConfigurationValidation.HasValidSmtpEndpoint,
        "Smtp:Host, Smtp:Port and Smtp:FromAddress must define a valid SMTP endpoint.")
    .Validate(ConfigurationValidation.HasMatchingSmtpCredentials,
        "Smtp:Username and Smtp:Password must either both be set or both be empty.")
    .ValidateOnStart();

builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "Jwt:Issuer is required.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "Jwt:Audience is required.")
    .Validate(options => Encoding.UTF8.GetByteCount(options.SigningKey ?? string.Empty) >= 32,
        "Jwt:SigningKey must contain at least 32 bytes.")
    .ValidateOnStart();
builder.Services.AddSingleton<IAccessTokenService, JwtAccessTokenService>();
builder.Services.AddSingleton<IAuthenticationLifetimeProvider, AuthenticationLifetimeProvider>();

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("identity-registration", httpContext =>
    {
        return CreateFixedWindowPartition(httpContext, 5, TimeSpan.FromMinutes(15));
    });

    options.AddPolicy("identity-email-verification", httpContext =>
    {
        return CreateFixedWindowPartition(httpContext, 20, TimeSpan.FromMinutes(1));
    });

    options.AddPolicy("identity-password-reset-request", httpContext =>
    {
        return CreateFixedWindowPartition(httpContext, 5, TimeSpan.FromMinutes(15));
    });

    options.AddPolicy("identity-password-reset-token", httpContext =>
    {
        return CreateFixedWindowPartition(httpContext, 20, TimeSpan.FromMinutes(1));
    });

    options.AddPolicy("identity-login", httpContext =>
    {
        return CreateFixedWindowPartition(httpContext, 10, TimeSpan.FromMinutes(15));
    });

    options.AddPolicy("identity-refresh", httpContext =>
    {
        return CreateFixedWindowPartition(httpContext, 30, TimeSpan.FromMinutes(1));
    });
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(IdentityAccessApplicationAssemblyReference).Assembly,
        typeof(PortfolioApplicationAssemblyReference).Assembly,
        typeof(LeasingApplicationAssemblyReference).Assembly,
        typeof(RentCollectionApplicationAssemblyReference).Assembly,
        typeof(TaxPreparationApplicationAssemblyReference).Assembly,
        typeof(MessagingApplicationAssemblyReference).Assembly);
});
builder.Services.AddIdentityAccessInfrastructure(builder.Configuration);
builder.Services.AddPortfolioInfrastructure(builder.Configuration);
builder.Services.AddLeasingInfrastructure(builder.Configuration);
builder.Services.AddRentCollectionInfrastructure(builder.Configuration);
builder.Services.AddTaxPreparationInfrastructure(builder.Configuration);
builder.Services.AddMessagingInfrastructure(builder.Configuration);

if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("LoyerisDatabase")))
    throw new InvalidOperationException("ConnectionStrings:LoyerisDatabase is required.");

var app = builder.Build();

if (args.Contains("--migrate", StringComparer.Ordinal))
{
    await DatabaseMigrator.MigrateAsync(app.Services, app.Logger, CancellationToken.None);
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseForwardedHeaders();
if (builder.Configuration.GetValue("HttpsRedirection:Enabled", true))
    app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.RegisterIdentityAccessEndpointGroup();
app.RegisterPortfolioEndpointGroup();
app.RegisterLeasingEndpointGroup();
app.RegisterRentCollectionEndpointGroup();
app.RegisterTaxPreparationEndpointGroup();
app.RegisterMessagingEndpointGroup();
app.RegisterHealthEndpointGroup();

app.Run();

static RateLimitPartition<string> CreateFixedWindowPartition(
    HttpContext httpContext,
    int permitLimit,
    TimeSpan window)
{
    return RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => CreateFixedWindowOptions(permitLimit, window));
}

static FixedWindowRateLimiterOptions CreateFixedWindowOptions(int permitLimit, TimeSpan window)
{
    return new FixedWindowRateLimiterOptions
    {
        PermitLimit = permitLimit,
        Window = window,
        QueueLimit = 0
    };
}
