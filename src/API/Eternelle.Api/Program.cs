using Eternelle.Api.Extensions;
using Eternelle.Api.Middleware;
using Eternelle.Api.OpenTelemetry;
using Eternelle.Common.Application;
using Eternelle.Common.Infrastructure;
using Eternelle.Common.Infrastructure.Configuration;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Modules.Weddings.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.Services.AddApplication([
    Eternelle.Modules.Weddings.Application.AssemblyReference.Assembly
]);

string databaseConnectionString = builder.Configuration.GetConnectionStringOrThrow("Database");
string redisConnectionString = builder.Configuration.GetConnectionStringOrThrow("Cache");

builder.Services.AddInfrastructure(
    DiagnosticsConfig.ServiceName,
    moduleConfigureConsumers: [],
    databaseConnectionString: databaseConnectionString,
    redisConnectionString: redisConnectionString);

builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString)
    .AddRedis(redisConnectionString);

builder.Configuration.AddModuleConfiguration(["weddings"]);

builder.Services.AddWeddingsModule(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.ApplyMigrations();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseLogContextTraceLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.MapEndpoints();

await app.RunAsync();

public partial class Program;
