using SensorMonitor.Core.DTOs;

namespace SensorMonitor.Core.Interfaces;

public interface IAnalyticsClient
{
    Task<PredictionResponse?> PredictAsync(
        PredictionRequest request, 
        CancellationToken ct = default);
}
