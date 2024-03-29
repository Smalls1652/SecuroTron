using Microsoft.Extensions.DependencyInjection;

using SecuroTron.Agent.Services;

namespace SecuroTron.Agent.Extensions.ServiceSetup;

/// <summary>
/// Extension methods for setting up <see cref="MainService"/>.
/// </summary>
internal static class MainServiceExtensions
{
    /// <summary>
    /// Adds a <see cref="MainService"/> service to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMainService(this IServiceCollection services)
    {
        services.AddHostedService<MainService>();

        return services;
    }
}