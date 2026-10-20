using System.Net.Http.Json;
using SensorMonitor.Core.DTOs;
using SensorMonitor.Core.Interfaces;

namespace SensorMonitor.Core.Services;

public class AnalyticsClient : IAnalyticsClient
{
    private readonly HttpClient _http;

    public AnalyticsClient(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("http://localhost:8000");
        _http.Timeout = TimeSpan.FromSeconds(5);
    }

    public async Task<PredictionResponse?> PredictAsync(
        PredictionRequest request, 
        CancellationToken ct = default)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("/predict", request, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PredictionResponse>(ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Analytics] Ошибка запроса: {ex.Message}");
            return null;
        }
    }
}
