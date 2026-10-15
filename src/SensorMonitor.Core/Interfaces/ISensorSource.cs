using SensorMonitor.Core.Models;

namespace SensorMonitor.Core.Interfaces;

/// <summary>
/// Источник данных с датчиков. Единый контракт для любого источника:
/// симулятор, MQTT-брокер, Serial-порт, REST API.
/// ViewModel и другие сервисы работают с интерфейсом, не зная,
/// откуда реально приходят данные.
/// </summary>
public interface ISensorSource
{
    /// <summary>
    /// Асинхронный поток показаний. Метод возвращает бесконечную
    /// последовательность SensorReading, которая прекращается
    /// при отмене через CancellationToken.
    /// </summary>
    IAsyncEnumerable<SensorReading> StreamAsync(CancellationToken ct = default);
}