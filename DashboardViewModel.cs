using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Services;

namespace FleetManager.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly AuthService _authService;
    private readonly DatabaseService _dbService;
    
    [ObservableProperty]
    private string _welcomeMessage = string.Empty;
    
    [ObservableProperty]
    private string _currentUserRole = string.Empty;
    
    [ObservableProperty]
    private int _totalVehicles;
    
    [ObservableProperty]
    private int _vehiclesInMaintenance;
    
    [ObservableProperty]
    private int _availableVehicles;
    
    [ObservableProperty]
    private double _averageFuelConsumption;
    
    [ObservableProperty]
    private int _totalMaintenanceCost;
    
    [ObservableProperty]
    private int _totalMileage;
    
    [ObservableProperty]
    private string _alerts = string.Empty;
    
    [ObservableProperty]
    private bool _hasAlerts;
    
    public DashboardViewModel(MainWindowViewModel mainViewModel, AuthService authService)
    {
        _mainViewModel = mainViewModel;
        _authService = authService;
        _dbService = new DatabaseService();
        
        if (UserSession.Instance.CurrentUser != null)
        {
            WelcomeMessage = $"Bienvenue, {UserSession.Instance.CurrentUser.Username}!";
            CurrentUserRole = UserSession.Instance.IsAdmin ? "Administrateur" : "Agent de bureau";
        }
        
        LoadData();
    }
    
    public bool IsAdmin => UserSession.Instance.IsAdmin;
    
    private async void LoadData()
    {
        TotalVehicles = await _dbService.GetTotalVehiclesAsync();
        VehiclesInMaintenance = await _dbService.GetVehiclesInMaintenanceAsync();
        AvailableVehicles = await _dbService.GetAvailableVehiclesAsync();
        AverageFuelConsumption = await _dbService.GetAverageFuelConsumptionAsync();
        TotalMaintenanceCost = await _dbService.GetTotalMaintenanceCostAsync();
        TotalMileage = await _dbService.GetTotalMileageAsync();
        
        var expiring = await _dbService.GetExpiringInsurancesAsync(30);
        if (expiring.Any())
        {
            HasAlerts = true;
            Alerts = $"{expiring.Count} assurance(s) expirent dans moins de 30 jours";
        }
    }
    
    [RelayCommand] private void GoToVehicles() => _mainViewModel.NavigateTo(new VehiclesViewModel(_mainViewModel, _authService, _dbService));
    [RelayCommand] private void GoToMaintenances() => _mainViewModel.NavigateTo(new MaintenancesViewModel(_mainViewModel, _dbService));
    [RelayCommand] private void GoToFuel() => _mainViewModel.NavigateTo(new FuelViewModel(_mainViewModel, _dbService));
    [RelayCommand] private void GoToProfile() => _mainViewModel.NavigateTo(new ProfileViewModel(_mainViewModel, _authService));
    [RelayCommand] private void GoToInsurances() => _mainViewModel.NavigateTo(new InsurancesViewModel(_mainViewModel, _dbService));
    [RelayCommand] private void GoToDrivers() => _mainViewModel.NavigateTo(new DriversViewModel(_mainViewModel, _dbService));
    [RelayCommand] private void GoToSchedules() => _mainViewModel.NavigateTo(new SchedulesViewModel(_mainViewModel, _dbService));
    [RelayCommand] private void GoToBudgets() => _mainViewModel.NavigateTo(new BudgetsViewModel(_mainViewModel, _dbService));
    [RelayCommand] private void GoToStatistics() => _mainViewModel.NavigateTo(new StatisticsViewModel(_mainViewModel, _dbService));
    [RelayCommand] private void GoToUsers() 
    { 
        if (UserSession.Instance.IsAdmin)
            _mainViewModel.NavigateTo(new UsersViewModel(_mainViewModel, _dbService));
    }
    [RelayCommand] private void Logout() { _authService.Logout(); _mainViewModel.NavigateTo(new LoginViewModel(_mainViewModel)); }
}
