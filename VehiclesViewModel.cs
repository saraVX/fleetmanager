using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Models;
using FleetManager.Services;

namespace FleetManager.ViewModels;

public partial class VehiclesViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly AuthService _authService;
    private readonly DatabaseService _dbService;
    
    [ObservableProperty]
    private ObservableCollection<Vehicle> _vehicles = new();
    
    [ObservableProperty]
    private Vehicle? _selectedVehicle;
    
    [ObservableProperty]
    private bool _isEditing;
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    [ObservableProperty]
    private string _formLicensePlate = string.Empty;
    
    [ObservableProperty]
    private string _formBrand = string.Empty;
    
    [ObservableProperty]
    private string _formModel = string.Empty;
    
    [ObservableProperty]
    private int _formYear = DateTime.Now.Year;
    
    [ObservableProperty]
    private string _formColor = string.Empty;
    
    [ObservableProperty]
    private int _formMileage = 0;
    
    [ObservableProperty]
    private string _formStatus = "Disponible";
    
    [ObservableProperty]
    private double _formFuelConsumption = 7.0;
    
    [ObservableProperty]
    private List<string> _statusList = new() { "Disponible", "En maintenance", "Hors service" };
    
    private List<Vehicle> _allVehicles = new();
    
    public VehiclesViewModel(MainWindowViewModel mainViewModel, AuthService authService, DatabaseService dbService)
    {
        _mainViewModel = mainViewModel;
        _authService = authService;
        _dbService = dbService;
        LoadVehicles();
    }
    
    private async void LoadVehicles()
    {
        _allVehicles = await _dbService.GetVehiclesAsync();
        Vehicles = new ObservableCollection<Vehicle>(_allVehicles);
    }
    
    partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Vehicles = new ObservableCollection<Vehicle>(_allVehicles);
        }
        else
        {
            var filtered = _allVehicles.Where(v => 
                v.LicensePlate.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                v.Brand.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                v.Model.Contains(value, StringComparison.OrdinalIgnoreCase)).ToList();
            Vehicles = new ObservableCollection<Vehicle>(filtered);
        }
    }
    
    [RelayCommand]
    private void NewVehicle()
    {
        IsEditing = true;
        ClearForm();
        SelectedVehicle = null;
    }
    
    [RelayCommand]
    private void EditVehicle()
    {
        if (SelectedVehicle == null) return;
        IsEditing = true;
        FormLicensePlate = SelectedVehicle.LicensePlate;
        FormBrand = SelectedVehicle.Brand;
        FormModel = SelectedVehicle.Model;
        FormYear = SelectedVehicle.Year;
        FormColor = SelectedVehicle.Color;
        FormMileage = SelectedVehicle.Mileage;
        FormStatus = SelectedVehicle.Status;
        FormFuelConsumption = SelectedVehicle.FuelConsumption;
    }
    
    [RelayCommand]
    private async Task SaveVehicleAsync()
    {
        if (SelectedVehicle != null && IsEditing)
        {
            SelectedVehicle.LicensePlate = FormLicensePlate;
            SelectedVehicle.Brand = FormBrand;
            SelectedVehicle.Model = FormModel;
            SelectedVehicle.Year = FormYear;
            SelectedVehicle.Color = FormColor;
            SelectedVehicle.Mileage = FormMileage;
            SelectedVehicle.Status = FormStatus;
            SelectedVehicle.FuelConsumption = FormFuelConsumption;
            await _dbService.UpdateVehicleAsync(SelectedVehicle);
        }
        else if (IsEditing)
        {
            var newVehicle = new Vehicle
            {
                LicensePlate = FormLicensePlate,
                Brand = FormBrand,
                Model = FormModel,
                Year = FormYear,
                Color = FormColor,
                Mileage = FormMileage,
                Status = FormStatus,
                FuelConsumption = FormFuelConsumption,
                CreatedAt = DateTime.Now
            };
            await _dbService.AddVehicleAsync(newVehicle);
        }
        
        IsEditing = false;
        LoadVehicles();
        ClearForm();
    }
    
    [RelayCommand]
    private async Task DeleteVehicleAsync()
    {
        if (SelectedVehicle == null) return;
        await _dbService.DeleteVehicleAsync(SelectedVehicle.Id);
        LoadVehicles();
        SelectedVehicle = null;
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
        var dashboard = new DashboardViewModel(_mainViewModel, _authService);
        _mainViewModel.NavigateTo(dashboard);
    }
    
    private void ClearForm()
    {
        FormLicensePlate = string.Empty;
        FormBrand = string.Empty;
        FormModel = string.Empty;
        FormYear = DateTime.Now.Year;
        FormColor = string.Empty;
        FormMileage = 0;
        FormStatus = "Disponible";
        FormFuelConsumption = 7.0;
        SelectedVehicle = null;
    }
}
