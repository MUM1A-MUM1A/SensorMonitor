namespace SensorMonitor.Core.Models;

/// <summary>
/// Статус состояния датчика или системы.
/// Используется в UI для цветовой индикации и в логике для алертов.
/// </summary>
public enum SensorStatus
{
    /// <summary>Всё в норме.</summary>
    Ok,

    /// <summary>Есть отклонения, стоит обратить внимание.</summary>
    Warning,

    /// <summary>Критическое состояние, требуется вмешательство.</summary>
    Critical
}