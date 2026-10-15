using System.Runtime.CompilerServices;
using SensorMonitor.Core.Interfaces;
using SensorMonitor.Core.Models;

namespace SensorMonitor.Core.Services;

/// <summary>
/// Режим работы симулятора.
/// </summary>
public enum SimulationMode
{
    /// <summary>Всё в норме, небольшие колебания вокруг базовых значений.</summary>
    Normal,

    /// <summary>Медленная деградация: температура и вибрация постепенно растут.</summary>
    Degrading,

    /// <summary>Быстрая порча: значения резко уходят в критические.</summary>
    Critical
}

/// <summary>
/// Симулятор датчиков. Генерирует реалистичные показания
/// с шумом, трендом и переключением режимов.
/// </summary>
public class SensorSimulator : ISensorSource
{
    private readonly Random _random = new();
    private readonly int _intervalMs;
    private readonly string _sensorId;

    // Базовые значения (то, к чему стремимся в текущем режиме)
    private double _baseTemp = 25.0;
    private double _baseHumidity = 50.0;
    private double _baseVibration = 0.3;

    /// <summary>
    /// Текущий режим. Присвой новое значение — симулятор начнёт
    /// плавно подстраиваться под новый режим.
    /// </summary>
    public SimulationMode Mode { get; set; } = SimulationMode.Normal;

    public SensorSimulator(
        int intervalMs = 1000,
        string sensorId = "sensor-01")
    {
        _intervalMs = intervalMs;
        _sensorId = sensorId;
    }

    public async IAsyncEnumerable<SensorReading> StreamAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            // 1. Обновляем базовые значения в зависимости от режима
            UpdateBaseValues();

            // 2. Добавляем шум и создаём показание
            var reading = new SensorReading
            {
                SensorId = _sensorId,
                Timestamp = DateTime.UtcNow,
                Temperature = Math.Round(_baseTemp + Noise(0.3), 2),
                Humidity = Math.Round(_baseHumidity + Noise(1.5), 2),
                Vibration = Math.Round(Math.Max(0, _baseVibration + Noise(0.03)), 3)
            };

            yield return reading;

            // 3. Ждём перед следующим показанием
            await Task.Delay(_intervalMs, ct);
        }
    }

    /// <summary>
    /// Обновляет базовые значения в зависимости от режима.
    /// Вызывается раз в секунду перед генерацией показания.
    /// </summary>
    private void UpdateBaseValues()
    {
        switch (Mode)
        {
            case SimulationMode.Normal:
                // Постепенно возвращаемся к норме
                _baseTemp += (25.0 - _baseTemp) * 0.05;
                _baseHumidity += (50.0 - _baseHumidity) * 0.05;
                _baseVibration += (0.3 - _baseVibration) * 0.05;
                break;

            case SimulationMode.Degrading:
                _baseTemp += 0.3;
                 _baseHumidity += 0.5;
                 _baseVibration += 0.02;
             break;
            case SimulationMode.Critical:
                _baseTemp += 1.5;
                _baseHumidity += 2.0;
                 _baseVibration += 0.1;
    break;
        }

        // Ограничения, чтобы значения не уходили в бесконечность
        _baseTemp = Math.Clamp(_baseTemp, -50, 150);
        _baseHumidity = Math.Clamp(_baseHumidity, 0, 100);
        _baseVibration = Math.Clamp(_baseVibration, 0, 10);
    }

    /// <summary>
    /// Возвращает случайное число в диапазоне [-amplitude, +amplitude].
    /// Это "шум" — небольшие колебания вокруг базового значения.
    /// </summary>
    private double Noise(double amplitude)
    {
        return (_random.NextDouble() * 2 - 1) * amplitude;
    }
}