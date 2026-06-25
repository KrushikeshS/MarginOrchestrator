namespace Orchestrator.Domain;

public interface IDisputeParser
{
    IEnumerable<MarginBreak> ParseDisputeCsv(string csvText);
}