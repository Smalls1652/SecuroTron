using Microsoft.Extensions.DependencyInjection;

using SecuroTron.Lib.Services;

namespace SecuroTron.Lib.Extensions.ServiceSetup;

/// <summary>
/// Extension methods for setting up <see cref="IActiveDirectoryService"/> services.
/// </summary>
public static class ActiveDirectoryServiceExtensions
{
    /// <summary>
    /// Adds an <see cref="IActiveDirectoryService"/> service to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="options">A delegate that is used to configure the <see cref="ActiveDirectoryServiceOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddActiveDirectoryService(this IServiceCollection services, Action<ActiveDirectoryServiceOptions> options)
    {
        services.Configure(options);

        services.AddTransient<IActiveDirectoryService, ActiveDirectoryService>();

        return services;
    }
}
