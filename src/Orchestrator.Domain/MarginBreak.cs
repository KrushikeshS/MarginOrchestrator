namespace Orchestrator.Domain;

public record MarginBreak(
    string DisputeId,
    string BrokerName,
    decimal BalanceDifference,
    DateTime ExecutionDate
);

