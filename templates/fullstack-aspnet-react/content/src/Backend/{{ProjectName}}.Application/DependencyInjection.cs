using Microsoft.Extensions.DependencyInjection;
using {{ProjectNamespace}}.Application.Status;

namespace {{ProjectNamespace}}.Application;

/// <summary>
/// Registers this layer's services. Each layer owns its own registrations so the
/// API's Program.cs stays a short list of AddX calls.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<StatusService>();

        return services;
    }
}
