using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orchestrator.Domain;

namespace Orchestrator.Worker;

public class IngestionWorker : BackgroundService
{
    private readonly ILogger<IngestionWorker> _logger;
    private readonly IDisputeParser _parser;
    private const string IngestionDirectory = "./ingest";

    public IngestionWorker(ILogger<IngestionWorker> logger, IDisputeParser parser)
    {
        _logger = logger;
        _parser = parser; // Injected via host dependency container
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Guarantee that watcher directory folder exists
        if (!Directory.Exists(IngestionDirectory))
        {
            Directory.CreateDirectory(IngestionDirectory);
        }

        _logger.LogInformation("Watching directory: {Dir}", IngestionDirectory);

        while (!stoppingToken.IsCancellationRequested)
        {
            // Search directory for broker CSV files
            var csvFiles = Directory.GetFiles(IngestionDirectory, "*.csv");

            foreach (var file in csvFiles)
            {
                _logger.LogInformation("Ingesting new file: {Path}", file);
                
                // Read CSV text asynchronously, yielding thread
                var text = await File.ReadAllTextAsync(file, stoppingToken);
                
                var breaks = _parser.ParseDisputeCsv(text);

                foreach (var item in breaks)
                {
                    _logger.LogInformation(
                        "Loaded MarginBreak: {DisputeId} | Broker: {Broker} | Out-of-Balance: ${Amount}",
                        item.DisputeId, item.BrokerName, item.BalanceDifference
                    );
                }

                // IDEMPOTENCY: Delete the file after reading so it isn't processed twice
                File.Delete(file);
            }

            // Sleep for 2 seconds before polling folder again
            await Task.Delay(2000, stoppingToken);
        }
    }
}