using Loyeris.Api.Endpoints;
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
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.Configure<ApplicationUrlOptions>(
    builder.Configuration.GetSection(ApplicationUrlOptions.SectionName));
builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection(SmtpOptions.SectionName));
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
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("identity-registration", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(15),
                QueueLimit = 0
            }));

    options.AddPolicy("identity-email-verification", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("identity-password-reset-request", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(15),
                QueueLimit = 0
            }));

    options.AddPolicy("identity-password-reset-token", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("identity-login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(15),
                QueueLimit = 0
            }));

    options.AddPolicy("identity-refresh", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(
        typeof(IdentityAccessApplicationAssemblyReference).Assembly,
        typeof(PortfolioApplicationAssemblyReference).Assembly,
        typeof(LeasingApplicationAssemblyReference).Assembly,
        typeof(RentCollectionApplicationAssemblyReference).Assembly,
        typeof(TaxPreparationApplicationAssemblyReference).Assembly,
        typeof(MessagingApplicationAssemblyReference).Assembly));
builder.Services.AddIdentityAccessInfrastructure(builder.Configuration);
builder.Services.AddPortfolioInfrastructure(builder.Configuration);
builder.Services.AddLeasingInfrastructure(builder.Configuration);
builder.Services.AddRentCollectionInfrastructure(builder.Configuration);
builder.Services.AddTaxPreparationInfrastructure(builder.Configuration);
builder.Services.AddMessagingInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

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

app.Run();
