using Loyeris.Api.Endpoints;
using Loyeris.IdentityAccess.App;
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
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.Configure<ApplicationUrlOptions>(
    builder.Configuration.GetSection(ApplicationUrlOptions.SectionName));
builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection(SmtpOptions.SectionName));
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

app.RegisterIdentityAccessEndpointGroup();
app.RegisterPortfolioEndpointGroup();
app.RegisterLeasingEndpointGroup();
app.RegisterRentCollectionEndpointGroup();
app.RegisterTaxPreparationEndpointGroup();
app.RegisterMessagingEndpointGroup();

app.Run();
