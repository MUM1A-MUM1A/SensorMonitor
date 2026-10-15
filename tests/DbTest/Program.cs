using SensorMonitor.Core.Models;
using SensorMonitor.Core.Services;

Console.WriteLine("Тест симулятора + БД \n");

// 1. Создаём симулятор и БД
var simulator = new SensorSimulator(intervalMs: 1000, sensorId: "sensor-01");

using (var db = new AppDbContext())
{
    db.Database.EnsureDeleted();   // ← удалить всё
    db.Database.EnsureCreated();
    Console.WriteLine("БД пересоздана.\n");
}

using (var db = new AppDbContext())
{
    db.Database.EnsureCreated();
}

Console.WriteLine(" Симулятор и БД готовы.\n");

// 2. Читаем 10 показаний и сохраняем в БД
var cts = new CancellationTokenSource();
int counter = 0;

try
{
    await foreach (var reading in simulator.StreamAsync(cts.Token))
    {
        counter++;

        Console.WriteLine(
            $"[{counter:D2}] {reading.SensorId} | " +
            $"T={reading.Temperature:F2}°C | " +
            $"H={reading.Humidity:F2}% | " +
            $"V={reading.Vibration:F3}");

        using (var db = new AppDbContext())
        {
            db.Readings.Add(reading);
            await db.SaveChangesAsync();
        }

        // Переключаем режимы для демонстрации
        if (counter == 3)
        {
            Console.WriteLine("\nПереключение в режим DEGRADING\n");
            simulator.Mode = SimulationMode.Degrading;
        }
        else if (counter == 7)
        {
            Console.WriteLine("\nПереключение в режим CRITICAL\n");
            simulator.Mode = SimulationMode.Critical;
        }
        else if (counter == 10)
        {
            Console.WriteLine("\n Стоп. Показаний достаточно.\n");
            cts.Cancel();
            break;
        }
    }
}
catch (OperationCanceledException)
{
    // Ожидаемое завершение
}

// 3. Проверяем, что всё сохранилось
using (var db = new AppDbContext())
{
    var total = db.Readings.Count();
    var last = db.Readings.OrderByDescending(r => r.Id).FirstOrDefault();

    Console.WriteLine($" Всего записей в БД: {total}");
    if (last != null)
    {
        Console.WriteLine(
            $"Последняя: T={last.Temperature:F2}°C, " +
            $"H={last.Humidity:F2}%, V={last.Vibration:F3}");
    }
}

Console.WriteLine("\nТест завершён");