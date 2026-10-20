using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SensorMonitor.App.Views;
using SensorMonitor.App.ViewModels;

namespace SensorMonitor.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        // Ловим все необработанные ошибки UI
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            File.WriteAllText("crash.txt", e.ExceptionObject.ToString());
        };
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try 
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(),
                };
            }
            catch (Exception ex)
            {
                File.WriteAllText("crash.txt", ex.ToString());
                throw; // Пробрасываем ошибку дальше
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}