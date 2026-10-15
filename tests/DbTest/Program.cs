using SensorMonitor.Core.Models;
using SensorMonitor.Core.Services;

Console.WriteLine("Тест базы данных\n");

// 1. Убеждаемся, что БД создана
using (var db = new AppDbContext())
{
    db.Database.EnsureCreated();
    Console.WriteLine("[OK] База данных готова.");
}

// 2. Записываем несколько показаний
using (var db = new AppDbContext())
{
    var readings = new[]
    {
        new SensorReading
        {
            SensorId = "sensor-01",
            Temperature = 25.4,
            Humidity = 48.2,
            Vibration = 0.32
        },
        new SensorReading
        {
            SensorId = "sensor-01",
            Temperature = 27.1,
            Humidity = 51.5,
            Vibration = 0.45,
            PredictedRisk = 0.15,
            PredictedStatus = SensorStatus.Ok
        },
        new SensorReading
        {
            SensorId = "sensor-02",
            Temperature = 31.8,
            Humidity = 68.0,
            Vibration = 0.81,
            PredictedRisk = 0.87,
            PredictedStatus = SensorStatus.Critical
        }
    };

    db.Readings.AddRange(readings);
    db.SaveChanges();

    Console.WriteLine($"[OK] Добавлено записей: {readings.Length}\n");
}

// 3. Читаем обратно
using (var db = new AppDbContext())
{
    var all = db.Readings
        .OrderByDescending(r => r.Timestamp)
        .ToList();

    Console.WriteLine($"=== Всего записей в БД: {all.Count} ===\n");

    foreach (var r in all)
    {
        Console.WriteLine(
            $"ID={r.Id} | {r.SensorId} | " +
            $"T={r.Temperature:F1}°C | H={r.Humidity:F1}% | V={r.Vibration:F2} | " +
            $"Risk={r.PredictedRisk?.ToString("P0") ?? "—"} | " +
            $"Status={r.PredictedStatus?.ToString() ?? "—"}");
    }

    Console.WriteLine();
}

// 4. Фильтрация
using (var db = new AppDbContext())
{
    var critical = db.Readings
        .Where(r => r.PredictedStatus == SensorStatus.Critical)
        .ToList();

    Console.WriteLine($"[OK] Критических показаний: {critical.Count}");
}

Console.WriteLine("\nТест завершён");