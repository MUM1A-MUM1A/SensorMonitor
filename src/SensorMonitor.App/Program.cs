using Avalonia;
using System;
using System.IO;

namespace SensorMonitor.App;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            // Если приложение падает, запишем ошибку прямо в консоль и в файл
            Console.WriteLine("=========================================");
            Console.WriteLine("ПРИЛОЖЕНИЕ УПАЛО С ОШИБКОЙ:");
            Console.WriteLine(ex.ToString());
            Console.WriteLine("=========================================");
            
            File.WriteAllText("crash.log", ex.ToString());
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}