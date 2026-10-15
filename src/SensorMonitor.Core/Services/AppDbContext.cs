using Microsoft.EntityFrameworkCore;
using SensorMonitor.Core.Models;

namespace SensorMonitor.Core.Services;

/// <summary>
/// Контекст базы данных. Точка входа для работы с SQLite.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Таблица с показаниями датчиков.
    /// </summary>
    public DbSet<SensorReading> Readings => Set<SensorReading>();

    /// <summary>
    /// Настройка подключения к базе. Здесь указываем путь к файлу SQLite.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "sensors.db");
            options.UseSqlite($"Data Source={dbPath}");
        }
    }

    /// <summary>
    /// Настройка модели данных: индексы, ограничения, связи.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SensorReading>()
            .HasIndex(r => r.Timestamp);

        modelBuilder.Entity<SensorReading>()
            .HasIndex(r => r.SensorId);
    }
}