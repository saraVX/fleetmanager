using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Services;

namespace FleetManager.ViewModels;

public partial class StatisticsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly DatabaseService _dbService;
    
    [ObservableProperty] private int _totalVehicles;
    [ObservableProperty] private int _availableVehicles;
    [ObservableProperty] private int _maintenanceVehicles;
    [ObservableProperty] private int _totalMaintenances;
    [ObservableProperty] private int _totalMaintenanceCost;
    [ObservableProperty] private int _totalFuelRecords;
    [ObservableProperty] private double _totalFuelLiters;
    [ObservableProperty] private int _totalFuelCost;
    [ObservableProperty] private int _totalMileage;
    [ObservableProperty] private double _averageFuelConsumption;
    [ObservableProperty] private double _averageMileage;
    [ObservableProperty] private int _totalInsurances;
    [ObservableProperty] private double _totalInsuranceCost;
    [ObservableProperty] private int _totalDrivers;
    [ObservableProperty] private int _upcomingMaintenances;
    [ObservableProperty] private int _expiringInsurances;
    
    public StatisticsViewModel(MainWindowViewModel mainViewModel, DatabaseService dbService)
    {
        _mainViewModel = mainViewModel;
        _dbService = dbService;
        LoadStatistics();
    }
    
    private async void LoadStatistics()
    {
        var vehicles = await _dbService.GetVehiclesAsync();
        TotalVehicles = vehicles.Count;
        AvailableVehicles = vehicles.Count(v => v.Status == "Disponible");
        MaintenanceVehicles = vehicles.Count(v => v.Status == "En maintenance");
        TotalMileage = vehicles.Sum(v => v.Mileage);
        AverageMileage = vehicles.Any() ? vehicles.Average(v => v.Mileage) : 0;
        AverageFuelConsumption = vehicles.Any() ? vehicles.Average(v => v.FuelConsumption) : 0;
        
        var maintenances = await _dbService.GetMaintenancesAsync();
        TotalMaintenances = maintenances.Count;
        TotalMaintenanceCost = maintenances.Sum(m => m.Cost);
        
        var fuelRecords = await _dbService.GetFuelRecordsAsync();
        TotalFuelRecords = fuelRecords.Count;
        TotalFuelLiters = fuelRecords.Sum(f => f.Liters);
        TotalFuelCost = (int)fuelRecords.Sum(f => f.Cost);
        
        var insurances = await _dbService.GetInsurancesAsync();
        TotalInsurances = insurances.Count;
        TotalInsuranceCost = insurances.Sum(i => i.Cost);
        ExpiringInsurances = insurances.Count(i => i.EndDate <= DateTime.Now.AddDays(30));
        
        var drivers = await _dbService.GetDriversAsync();
        TotalDrivers = drivers.Count;
        
        var schedules = await _dbService.GetMaintenanceSchedulesAsync();
        UpcomingMaintenances = schedules.Count(s => !s.IsDone && s.ScheduledDate <= DateTime.Now.AddDays(15));
    }
    
    [RelayCommand]
    private void GoBack()
    {
        var authService = new AuthService();
        var dashboard = new DashboardViewModel(_mainViewModel, authService);
        _mainViewModel.NavigateTo(dashboard);
    }
}
