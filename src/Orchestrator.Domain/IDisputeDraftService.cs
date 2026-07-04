using System.Threading.Tasks;

namespace Orchestrator.Domain;

public interface IDisputeDraftService
{
    Task<string> GenerateVerifiedDraftAsync(MarginBreak marginBreak, string rootCause);
}