using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestrator.Domain;
using Orchestrator.Infrastructure;
using Orchestrator.Worker;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    // Bind CSV implementation to IDisputeParser interface
    services.AddSingleton<IDisputeParser, CsvDisputeParser>();

    // Register background worker daemon
    services.AddHostedService<IngestionWorker>();
});

var host = builder.Build();
await host.RunAsync();