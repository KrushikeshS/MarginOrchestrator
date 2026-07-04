using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Orchestrator.Domain;
using Orchestrator.Infrastructure;
using Orchestrator.Worker;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    services.AddSingleton<IDisputeParser, CsvDisputeParser>();

    // Register Semantic Kernel instance with a completion service config
    services.AddSingleton<Kernel>(sp =>
    {
        var kernelBuilder = Kernel.CreateBuilder();
        
        // Example adding Azure OpenAI Service:
        // kernelBuilder.AddAzureOpenAIChatCompletion("deploymentName", "endpoint", "apiKey");
        
        return kernelBuilder.Build();
    });

    services.AddSingleton<IDisputeDraftService, SemanticKernelDraftService>();
    services.AddHostedService<IngestionWorker>();
});

var host = builder.Build();
await host.RunAsync();