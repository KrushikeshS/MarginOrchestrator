using Microsoft.ML.Data;

namespace Orchestrator.Infrastructure.Models;

public class DisputeModelInput
{
    [LoadColumn(0)]
    public string BrokerName { get; set; } = string.Empty;

    [LoadColumn(1)]
    public float BalanceDifference { get; set; }

    [LoadColumn(2)]
    public float DisputeAgeDays { get; set; }

    [LoadColumn(3)]
    public string ExecutionDayOfWeek { get; set; } = string.Empty;
}