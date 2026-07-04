using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ML;
using Orchestrator.Domain;
using Orchestrator.Infrastructure;
using Orchestrator.Infrastructure.Models;
using Orchestrator.Worker;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    services.AddSingleton<IDisputeParser, CsvDisputeParser>();

    // 1. Register PredictionEnginePool pointing to model.zip path
    services.AddPredictionEnginePool<DisputeModelInput, DisputeModelOutput>()
        .FromFile("model.zip");

    // 2. Bind the domain classifier interface to the ML.NET implementation
    services.AddSingleton<IDisputeClassifier, MlNetDisputeClassifier>();

    services.AddHostedService<IngestionWorker>();
});

var host = builder.Build();
await host.RunAsync();