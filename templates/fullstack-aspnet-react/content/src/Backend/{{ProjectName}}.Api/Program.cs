using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using {{ProjectNamespace}}.Api.Diagnostics;
using {{ProjectNamespace}}.Api.Hosting;
using {{ProjectNamespace}}.Application;
using {{ProjectNamespace}}.Application.Status;
using {{ProjectNamespace}}.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuration comes from appsettings*.json and environment variables.
// Environment variables win, which is how Docker and production supply secrets:
// ConnectionStrings__Default maps to the "ConnectionStrings:Default" key.
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Unhandled exceptions become RFC 7807 responses instead of leaking stack traces.
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<IEnvironmentInfo, HostEnvironmentInfo>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(
        DatabaseHealthCheck.Name,
        tags: [HealthCheckTags.Ready]);

builder.Services.AddCors(options => options.AddPolicy(
    CorsPolicies.LocalDevelopment,
    policy => policy
        .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(CorsPolicies.LocalDevelopment);
}

// Liveness: the process is up. Readiness: it can also reach its dependencies.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains(HealthCheckTags.Ready),
});

app.MapControllers();

app.Run();

/// <summary>
/// Exposed so the integration test project can host the API with
/// <c>WebApplicationFactory&lt;Program&gt;</c>.
/// </summary>
public partial class Program;
