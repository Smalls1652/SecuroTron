using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecuroTron.Hosting.Extensions;
using SecuroTron.Lib.Extensions.ServiceSetup;
using SecuroTron.Agent.Extensions.ServiceSetup;
using SecuroTron.Agent.Services;
using SecuroTron.Agent.Services.TaskQueues;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSlimHostLifetime();

builder.Configuration
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

            options.QueueName = builder.Configuration.GetValue<string>("QUEUE_NAME") ?? throw new NullReferenceException("QUEUE_NAME is required.");
        }
        else
        {
            options.EndpointUri = builder.Configuration.GetValue<Uri>("QUEUE_ENDPOINT_URI") ?? throw new NullReferenceException("QUEUE_ENDPOINT_URI is required.");

            options.QueueName = builder.Configuration.GetValue<string>("QUEUE_NAME") ?? throw new NullReferenceException("QUEUE_NAME is required.");
        }
    });

builder.Services
    .AddActiveDirectoryService(options =>
    {
        options.ServerFqdn = builder.Configuration.GetValue<string>("AD_SERVER_FQDN") ?? throw new NullReferenceException("AD_SERVER_FQDN is required.");

        options.Username = builder.Configuration.GetValue<string>("AD_USERNAME") ?? throw new NullReferenceException("AD_USERNAME is required.");

        options.Password = builder.Configuration.GetValue<string>("AD_PASSWORD") ?? throw new NullReferenceException("AD_PASSWORD is required.");

        options.DomainName = builder.Configuration.GetValue<string>("AD_DOMAIN_NAME") ?? throw new NullReferenceException("AD_DOMAIN_NAME is required.");

        options.RootDistiguishedName = builder.Configuration.GetValue<string>("AD_ROOT_DN") ?? throw new NullReferenceException("AD_ROOT_DN is required.");
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

await app.RunAsync();
