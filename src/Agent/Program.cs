using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SecuroTron.Agent;
using SecuroTron.Agent.Extensions.ServiceSetup;
using SecuroTron.Agent.Services;
using SecuroTron.Agent.Services.TaskQueues;
using SecuroTron.Hosting.Extensions;
using SecuroTron.Lib.Extensions.ServiceSetup;

using ILoggerFactory appLoggerFactory = LoggerFactory.Create(options =>
{
    options.AddSimpleConsole();
});

ILogger appLogger = appLoggerFactory.CreateLogger("SecuroTron.Agent");

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSlimHostLifetime();

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        path: "appsettings.json",
        optional: true,
        reloadOnChange: true
    )
    .AddJsonFile(
        path: $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true
    )
    .AddEnvironmentVariables()
    .AddCommandLine(args);

builder.Services
    .AddQueueClientService(options =>
    {
        if (builder.Configuration.GetValue<string>("QUEUE_CONNECTION_STRING") is not null)
        {
            options.ConnectionString = builder.Configuration.GetValue<string>("QUEUE_CONNECTION_STRING");

            options.QueueName = builder.Configuration.GetValue<string>("QUEUE_NAME") ?? throw new MissingConfigurationValueException("QUEUE_NAME");
        }
        else
        {
            options.EndpointUri = builder.Configuration.GetValue<Uri>("QUEUE_ENDPOINT_URI") ?? throw new MissingConfigurationValueException("QUEUE_ENDPOINT_URI");

            options.QueueName = builder.Configuration.GetValue<string>("QUEUE_NAME") ?? throw new MissingConfigurationValueException("QUEUE_NAME");
        }
    });

builder.Services
    .AddActiveDirectoryService(options =>
    {
        options.ServerFqdn = builder.Configuration.GetValue<string>("AD_SERVER_FQDN") ?? throw new MissingConfigurationValueException("AD_SERVER_FQDN");

        options.Username = builder.Configuration.GetValue<string>("AD_USERNAME") ?? throw new MissingConfigurationValueException("AD_USERNAME");

        options.Password = builder.Configuration.GetValue<string>("AD_PASSWORD") ?? throw new MissingConfigurationValueException("AD_PASSWORD");

        options.DomainName = builder.Configuration.GetValue<string>("AD_DOMAIN_NAME") ?? throw new MissingConfigurationValueException("AD_DOMAIN_NAME");

        options.RootDistiguishedName = builder.Configuration.GetValue<string>("AD_ROOT_DN") ?? throw new MissingConfigurationValueException("AD_ROOT_DN");
    });

builder.Services
    .AddSingleton<IBackgroundTaskQueue>(_ =>
    {
        return new DefaultBackgroundTaskQueue(100);
    });

builder.Services
    .AddSingleton<ITaskMonitorService, TaskMonitorService>();

builder.Services
    .AddMainService();

var app = builder.Build();

try
{
    await app.RunAsync();
}
catch (MissingConfigurationValueException ex)
{
    appLogger.LogError(
        exception: null,
        message: "{Message}",
        args: [
            ex.Message
        ]
    );

    return 1;
}
catch (Exception)
{
    return 1;
}

return 0;