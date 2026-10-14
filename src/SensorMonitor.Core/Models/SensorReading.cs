namespace SensorMonitor.Core.Models;

public class SensorReading
{
    /// <summary>Уникальный идентификатор записи в базе данных.</summary>
    public long Id { get; set; }

    /// <summary>Идентификатор датчика, с которого пришло показание (например, "sensor-01").</summary>
    public string SensorId { get; set; } = string.Empty;

    /// <summary>Момент времени, когда было снято показание.</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

   
    public double Temperature { get; set; }

    /// <summary>Относительная влажность в процентах (0-100).</summary>
    public double Humidity { get; set; }

   
    public double Vibration { get; set; }

    /// <summary>Оценка риска от ML-модели (0.0 - 1.0). Null — ещё не рассчитано.</summary>
    public double? PredictedRisk { get; set; }

    /// <summary>Статус от ML-модели. Null — ещё не рассчитано.</summary>
    public SensorStatus? PredictedStatus { get; set; }
}