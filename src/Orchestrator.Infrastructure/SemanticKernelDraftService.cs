using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Orchestrator.Domain;

namespace Orchestrator.Infrastructure;

public class SemanticKernelDraftService : IDisputeDraftService
{
    private readonly Kernel _kernel;

    public SemanticKernelDraftService(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<string> GenerateVerifiedDraftAsync(MarginBreak breakInfo, string rootCause)
    {
        var chat = _kernel.GetRequiredService<IChatCompletionService>();
        
        string actorPrompt = $"Write a professional dispute notification to {breakInfo.BrokerName} regarding break {breakInfo.DisputeId} " +
                             $"caused by a {rootCause}. Out-of-balance amount is ${breakInfo.BalanceDifference}. Cite standard ISDA terms.";
        
        string draft = "";
        bool isApproved = false;
        int maxRetries = 3;
        int attempts = 0;

        while (!isApproved && attempts < maxRetries)
        {
            attempts++;
            
            // 1. Actor generates draft
            var response = await chat.GetChatMessageContentAsync(actorPrompt);
            draft = response.Content ?? "";

            // 2. Critic evaluates draft
            var criticChat = new ChatHistory();
            criticChat.AddSystemMessage("You are an expert compliance auditor. Verify if the email draft includes the correct " +
                                        $"DisputeId ({breakInfo.DisputeId}) and exact amount (${breakInfo.BalanceDifference}). " +
                                        "Respond with ONLY 'APPROVED' or list the missing/incorrect values.");
            criticChat.AddUserMessage(draft);

            var evaluation = await chat.GetChatMessageContentAsync(criticChat);
            string evaluationText = evaluation.ToString().Trim();

            if (evaluationText.Contains("APPROVED"))
            {
                isApproved = true;
            }
            else
            {
                // Feed Critic feedback back to Actor for revision
                actorPrompt += $"\n\nCorrection required by compliance reviewer: {evaluationText}. Please rewrite accordingly.";
            }
        }

        return draft;
    }
}