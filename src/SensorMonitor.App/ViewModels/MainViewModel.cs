using CommunityToolkit.Mvvm.ComponentModel;

namespace SensorMonitor.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
}
