using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SensorMonitor.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] 
    private ViewModelBase _currentPage;

    public MainViewModel() 
    { 
        CurrentPage = new DashboardViewModel(); 
    }

    [RelayCommand] 
    private void ShowDashboard() => CurrentPage = new DashboardViewModel();
    
    [RelayCommand] 
    private void ShowCharts() => CurrentPage = new ChartsViewModel();
    
    [RelayCommand] 
    private void ShowSettings() => CurrentPage = new SettingsViewModel();
}