using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitor.Core.Models;
using SensorMonitor.Core.Services;
using SensorMonitor.Core.DTOs;

namespace SensorMonitor.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // --- Свойства для карточек (цифры на экране) ---
    [ObservableProperty] private string _temperature = "0.0°C";
    [ObservableProperty] private string _humidity = "0%";
    [ObservableProperty] private string _risk = "0%";
    [ObservableProperty] private string _statusColor = "#A6E3A1"; // Зеленый по умолчанию

    private readonly SensorSimulator _simulator;
    private readonly AnalyticsClient _analytics;

    public MainViewModel()
    {
        // Инициализация логики
        _simulator = new SensorSimulator(intervalMs: 1000, sensorId: "sensor-01");
        _analytics = new AnalyticsClient(new HttpClient());

        // Запускаем бесконечный цикл в фоне
        _ = StartMonitoringAsync();
    }

    private async Task StartMonitoringAsync()
    {
        var cts = new CancellationTokenSource();

        try
        {
            // Подключаемся к БД
            using (var db = new AppDbContext()) { db.Database.EnsureCreated(); }

            await foreach (var reading in _simulator.StreamAsync(cts.Token))
            {
                // 1. Отправляем в Python
                var request = new PredictionRequest 
                { 
                    Temperature = reading.Temperature, 
                    Humidity = reading.Humidity, 
                    Vibration = reading.Vibration 
                };
                
                var prediction = await _analytics.PredictAsync(request);

                // 2. Обновляем карточки на экране
                Avalonia.Threading.Dispatcher.UIThread.Invoke(() =>
                {
                    Temperature = $"{reading.Temperature:F1}°C";
                    Humidity = $"{reading.Humidity:F0}%";

                    if (prediction != null)
                    {
                        Risk = $"{prediction.Risk:P0}";
                        
                        // Меняем цвет карточки риска
                        StatusColor = prediction.Status switch
                        {
                            "ok" => "#A6E3A1",       // Зеленый
                            "warning" => "#F9E2AF",  // Желтый
                            "critical" => "#F38BA8", // Красный
                            _ => "#89B4FA"
                        };
                    }
                });

                // 3. Сохраняем в БД
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
                    using var db = new AppDbContext();
                    db.Readings.Add(reading);
                    await db.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}