using Eternelle.Api.Extensions;
using Eternelle.Api.Middleware;
using Eternelle.Api.OpenTelemetry;
using Eternelle.Common.Application;
using Eternelle.Common.Infrastructure;
using Eternelle.Common.Infrastructure.Configuration;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Modules.Weddings.Infrastructure;
using Eternelle.Modules.Weddings.Presentation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using Serilog;
using System.Threading.RateLimiting;

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

builder.Services.AddRateLimiter(options =>
{
    // IP-based fixed-window limiter for the public guest photo upload endpoint.
    // 10 uploads per minute per IP — protects against spam without a JWT to identify callers.
    options.AddPolicy(RateLimitingPolicies.GuestPhotoUpload, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

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

app.UseRateLimiter();

app.MapEndpoints();

await app.RunAsync();

public partial class Program;
