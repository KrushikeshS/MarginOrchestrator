using Microsoft.Extensions.ML;
using Orchestrator.Domain;
using Orchestrator.Infrastructure.Models;

namespace Orchestrator.Infrastructure;

public class MlNetDisputeClassifier : IDisputeClassifier
{
    private readonly PredictionEnginePool<DisputeModelInput, DisputeModelOutput> _predictionEnginePool;

    public MlNetDisputeClassifier(PredictionEnginePool<DisputeModelInput, DisputeModelOutput> pool)
    {
        _predictionEnginePool = pool;
    }

    public string ClassifyRootCause(MarginBreak marginBreak)
    {
        // Map Domain MarginBreak model to ML.NET Input Schema
        var input = new DisputeModelInput
        {
            BrokerName = marginBreak.BrokerName,
            BalanceDifference = (float)marginBreak.BalanceDifference,
            DisputeAgeDays = 0f, // Defaulting initial age
            ExecutionDayOfWeek = marginBreak.ExecutionDate.DayOfWeek.ToString()
        };

        // Use pooled, thread-safe prediction execution
        var prediction = _predictionEnginePool.Predict(input);

        return string.IsNullOrEmpty(prediction.PredictedLabel) 
            ? "Unknown Classification" 
            : prediction.PredictedLabel;
    }
}