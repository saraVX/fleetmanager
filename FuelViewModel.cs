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

public partial class FuelViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly DatabaseService _dbService;
    
    [ObservableProperty]
    private ObservableCollection<FuelRecord> _fuelRecords = new();
    
    [ObservableProperty]
    private ObservableCollection<Vehicle> _vehicles = new();
    
    [ObservableProperty]
    private bool _isAdding;
    
    [ObservableProperty]
    private int _formVehicleId;
    
    [ObservableProperty]
    private string _formVehicleName = string.Empty;
    
    [ObservableProperty]
    private DateTime _formRefuelDate = DateTime.Now;
    
    [ObservableProperty]
    private double _formLiters;
    
    [ObservableProperty]
    private double _formCost;
    
    [ObservableProperty]
    private int _formMileage;
    
    [ObservableProperty]
    private string _message = string.Empty;
    
    private List<FuelRecord> _allFuelRecords = new();
    
    public FuelViewModel(MainWindowViewModel mainViewModel, DatabaseService dbService)
    {
        _mainViewModel = mainViewModel;
        _dbService = dbService;
        LoadData();
    }
    
    private async void LoadData()
    {
        _allFuelRecords = await _dbService.GetFuelRecordsAsync();
        FuelRecords = new ObservableCollection<FuelRecord>(_allFuelRecords);
        Vehicles = new ObservableCollection<Vehicle>(await _dbService.GetVehiclesAsync());
    }
    
    [RelayCommand]
    private void ShowAddForm()
    {
        IsAdding = true;
        ClearForm();
    }
    
    [RelayCommand]
    private async Task SaveFuelRecordAsync()
    {
        if (FormVehicleId == 0)
        {
            Message = "Veuillez selectionner un vehicule";
            return;
        }
        if (FormLiters <= 0 || FormCost <= 0)
        {
            Message = "Veuillez entrer des valeurs valides";
            return;
        }
        
        var newRecord = new FuelRecord
        {
            VehicleId = FormVehicleId,
            RefuelDate = FormRefuelDate,
            Liters = FormLiters,
            Cost = FormCost,
            Mileage = FormMileage
        };
        await _dbService.AddFuelRecordAsync(newRecord);
        IsAdding = false;
        Message = "Plein ajoute avec succes !";
        LoadData();
        ClearForm();
    }
    
    [RelayCommand]
    private void CancelAdd()
    {
        IsAdding = false;
        ClearForm();
        Message = string.Empty;
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
        FormVehicleName = string.Empty;
        FormRefuelDate = DateTime.Now;
        FormLiters = 0;
        FormCost = 0;
        FormMileage = 0;
    }
}
