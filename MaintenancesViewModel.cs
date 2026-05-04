using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Models;
using FleetManager.Services;

namespace FleetManager.ViewModels;

public partial class MaintenancesViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly DatabaseService _dbService;
    
    [ObservableProperty]
    private ObservableCollection<Maintenance> _maintenances = new();
    
    [ObservableProperty]
    private ObservableCollection<Vehicle> _vehicles = new();
    
    [ObservableProperty]
    private Maintenance? _selectedMaintenance;
    
    [ObservableProperty]
    private bool _isEditing;
    
    [ObservableProperty]
    private int _formVehicleId;
    
    [ObservableProperty]
    private DateTime _formMaintenanceDate = DateTime.Now;
    
    [ObservableProperty]
    private string _formMaintenanceType = string.Empty;
    
    [ObservableProperty]
    private string _formDescription = string.Empty;
    
    [ObservableProperty]
    private int _formCost;
    
    [ObservableProperty]
    private int _formMileageAtMaintenance;
    
    [ObservableProperty]
    private string _formMechanic = string.Empty;
    
    public MaintenancesViewModel(MainWindowViewModel mainViewModel, DatabaseService dbService)
    {
        _mainViewModel = mainViewModel;
        _dbService = dbService;
        LoadData();
    }
    
    private async void LoadData()
    {
        Maintenances = new ObservableCollection<Maintenance>(await _dbService.GetMaintenancesAsync());
        Vehicles = new ObservableCollection<Vehicle>(await _dbService.GetVehiclesAsync());
    }
    
    [RelayCommand]
    private void NewMaintenance()
    {
        IsEditing = true;
        ClearForm();
        SelectedMaintenance = null;
    }
    
    [RelayCommand]
    private void EditMaintenance()
    {
        if (SelectedMaintenance == null) return;
        IsEditing = true;
        FormVehicleId = SelectedMaintenance.VehicleId;
        FormMaintenanceDate = SelectedMaintenance.MaintenanceDate;
        FormMaintenanceType = SelectedMaintenance.MaintenanceType;
        FormDescription = SelectedMaintenance.Description;
        FormCost = SelectedMaintenance.Cost;
        FormMileageAtMaintenance = SelectedMaintenance.MileageAtMaintenance;
        FormMechanic = SelectedMaintenance.Mechanic;
    }
    
    [RelayCommand]
    private async Task SaveMaintenanceAsync()
    {
        if (SelectedMaintenance != null && IsEditing)
        {
            SelectedMaintenance.VehicleId = FormVehicleId;
            SelectedMaintenance.MaintenanceDate = FormMaintenanceDate;
            SelectedMaintenance.MaintenanceType = FormMaintenanceType;
            SelectedMaintenance.Description = FormDescription;
            SelectedMaintenance.Cost = FormCost;
            SelectedMaintenance.MileageAtMaintenance = FormMileageAtMaintenance;
            SelectedMaintenance.Mechanic = FormMechanic;
            await _dbService.UpdateMaintenanceAsync(SelectedMaintenance);
        }
        else if (IsEditing)
        {
            var newMaintenance = new Maintenance
            {
                VehicleId = FormVehicleId,
                MaintenanceDate = FormMaintenanceDate,
                MaintenanceType = FormMaintenanceType,
                Description = FormDescription,
                Cost = FormCost,
                MileageAtMaintenance = FormMileageAtMaintenance,
                Mechanic = FormMechanic
            };
            await _dbService.AddMaintenanceAsync(newMaintenance);
        }
        
        IsEditing = false;
        LoadData();
        ClearForm();
    }
    
    [RelayCommand]
    private async Task DeleteMaintenanceAsync()
    {
        if (SelectedMaintenance == null) return;
        await _dbService.DeleteMaintenanceAsync(SelectedMaintenance.Id);
        LoadData();
        SelectedMaintenance = null;
    }
    
    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        ClearForm();
    }
    
    [RelayCommand]
    private void GoBack()
    {
        var authService = new AuthService();
        var dashboard = new DashboardViewModel(_mainViewModel, authService);
        _mainViewModel.NavigateTo(dashboard);
    }
    
    private void ClearForm()
    {
        FormVehicleId = 0;
        FormMaintenanceDate = DateTime.Now;
        FormMaintenanceType = string.Empty;
        FormDescription = string.Empty;
        FormCost = 0;
        FormMileageAtMaintenance = 0;
        FormMechanic = string.Empty;
        SelectedMaintenance = null;
    }
}
