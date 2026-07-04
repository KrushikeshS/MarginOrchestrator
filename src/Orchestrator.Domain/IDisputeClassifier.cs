namespace Orchestrator.Domain;

public interface IDisputeClassifier
{
    string ClassifyRootCause(MarginBreak marginBreak);
}