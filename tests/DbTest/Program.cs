using SensorMonitor.Core.Models;
using SensorMonitor.Core.Services;
using SensorMonitor.Core.DTOs;

Console.WriteLine("=== ТЕСТ СВЯЗИ C# -> PYTHON ===\n");

var simulator = new SensorSimulator(intervalMs: 1000, sensorId: "sensor-01");
var httpClient = new HttpClient();
var analytics = new AnalyticsClient(httpClient);

using (var db = new AppDbContext())
{
    db.Database.EnsureCreated();
}

var cts = new CancellationTokenSource();
int counter = 0;

try
{
    Console.WriteLine("Ожидание данных...\n");

    await foreach (var reading in simulator.StreamAsync(cts.Token))
    {
        counter++;

        var request = new PredictionRequest
        {
            Temperature = reading.Temperature,
            Humidity = reading.Humidity,
            Vibration = reading.Vibration
        };

        // СТУЧИМСЯ В PYTHON
        var prediction = await analytics.PredictAsync(request);

        if (prediction != null)
        {
            reading.PredictedRisk = prediction.Risk;
            reading.PredictedStatus = prediction.Status switch
            {
                "ok" => SensorStatus.Ok,
                "warning" => SensorStatus.Warning,
                "critical" => SensorStatus.Critical,
                _ => null
            };
        }

        using (var db = new AppDbContext())
        {
            db.Readings.Add(reading);
            await db.SaveChangesAsync();
        }

        string riskStr = prediction != null 
            ? $"{prediction.Risk:P0} ({prediction.Status})" 
            : "PYTHON НЕДОСТУПЕН";
            
        Console.WriteLine($"[{counter:D2}] T={reading.Temperature:F1}°C | " +
                          $"Python ответил: {riskStr}");

        if (counter == 3) simulator.Mode = SimulationMode.Degrading;
        if (counter == 6) simulator.Mode = SimulationMode.Critical;
        if (counter == 10)
        {
            cts.Cancel();
            break;
        }
    }
}
catch (OperationCanceledException) { }

Console.WriteLine("\n=== Тест завершен ===");