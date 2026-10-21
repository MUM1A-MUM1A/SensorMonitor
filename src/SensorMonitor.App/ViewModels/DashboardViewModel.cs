using System;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SensorMonitor.Core.Models;
using SensorMonitor.Core.Services;
using SensorMonitor.Core.DTOs;

namespace SensorMonitor.App.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    [ObservableProperty] private string _temperature = "0.0°C";
    [ObservableProperty] private string _humidity = "0%";
    [ObservableProperty] private string _risk = "0%";
    [ObservableProperty] private string _statusColor = "#A6E3A1";

    public ISeries[] Series { get; set; }
    public ObservableCollection<double> TempValues { get; } = new();

    private readonly SensorSimulator _simulator;
    private readonly AnalyticsClient _analytics;

    public DashboardViewModel()
    {
        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = TempValues,
                Fill = new SolidColorPaint(SKColors.DodgerBlue.WithAlpha(50)),
                Stroke = new SolidColorPaint(SKColors.DodgerBlue, 3),
                GeometrySize = 0
            }
        };

        _simulator = new SensorSimulator(intervalMs: 1000, sensorId: "sensor-01");
        _analytics = new AnalyticsClient(new HttpClient());
        _ = StartMonitoringAsync();
    }

    private async Task StartMonitoringAsync()
    {
        var cts = new CancellationTokenSource();
        try
        {
            using (var db = new AppDbContext()) { db.Database.EnsureCreated(); }
            await foreach (var reading in _simulator.StreamAsync(cts.Token))
            {
                var request = new PredictionRequest { Temperature = reading.Temperature, Humidity = reading.Humidity, Vibration = reading.Vibration };
                var prediction = await _analytics.PredictAsync(request);

                Avalonia.Threading.Dispatcher.UIThread.Invoke(() =>
                {
                    Temperature = $"{reading.Temperature:F1}°C";
                    Humidity = $"{reading.Humidity:F0}%";
                    if (prediction != null)
                    {
                        Risk = $"{prediction.Risk:P0}";
                        StatusColor = prediction.Status switch { "ok" => "#A6E3A1", "warning" => "#F9E2AF", "critical" => "#F38BA8", _ => "#89B4FA" };
                    }
                    TempValues.Add(reading.Temperature);
                    if (TempValues.Count > 30) TempValues.RemoveAt(0);
                });

                if (prediction != null)
                {
                    reading.PredictedRisk = prediction.Risk;
                    reading.PredictedStatus = prediction.Status switch { "ok" => SensorStatus.Ok, "warning" => SensorStatus.Warning, "critical" => SensorStatus.Critical, _ => null };
                    using var db = new AppDbContext();
                    db.Readings.Add(reading);
                    await db.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }
    }
}
