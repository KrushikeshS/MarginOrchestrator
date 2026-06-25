namespace Orchestrator.Domain;

public record MarginBreak(
    string DisputeID,
    string BrokerName,
    decimal BalanceDifference,
    DateTime ExecutionDate
);

