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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
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

app.RegisterIdentityAccessEndpointGroup();
app.RegisterPortfolioEndpointGroup();
app.RegisterLeasingEndpointGroup();
app.RegisterRentCollectionEndpointGroup();
app.RegisterTaxPreparationEndpointGroup();
app.RegisterMessagingEndpointGroup();

app.Run();
