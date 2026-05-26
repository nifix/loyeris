using Loyeris.Api.Endpoints;
using Loyeris.Auth.App.Queries;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetAuthHelloWorldQuery).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// RegisterAuthEndpoints
app.RegisterAuthEndpointGroup(app.Services.GetRequiredService<IMediator>());

app.Run();
