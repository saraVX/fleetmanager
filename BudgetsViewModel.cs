using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Models;
using FleetManager.Services;

namespace FleetManager.ViewModels;

public partial class BudgetsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly DatabaseService _dbService;
    
    [ObservableProperty] private ObservableCollection<Budget> _budgets = new();
    [ObservableProperty] private Budget? _selectedBudget;
    [ObservableProperty] private bool _isEditing;
    [ObservableProperty] private int _formVehicleId;
    [ObservableProperty] private int _formMonth = DateTime.Now.Month;
    [ObservableProperty] private int _formYear = DateTime.Now.Year;
    [ObservableProperty] private double _formPlannedAmount;
    [ObservableProperty] private double _formActualAmount;
    
    public BudgetsViewModel(MainWindowViewModel mainViewModel, DatabaseService dbService)
    {
        _mainViewModel = mainViewModel;
        _dbService = dbService;
        LoadData();
    }
    
    private async void LoadData() => Budgets = new ObservableCollection<Budget>(await _dbService.GetBudgetsAsync());
    
    [RelayCommand] private void New() { IsEditing = true; ClearForm(); }
    [RelayCommand] private async Task Save() { await _dbService.AddBudgetAsync(new Budget { VehicleId = FormVehicleId, Month = FormMonth, Year = FormYear, PlannedAmount = FormPlannedAmount, ActualAmount = FormActualAmount }); IsEditing = false; LoadData(); ClearForm(); }
    [RelayCommand] private void Cancel() { IsEditing = false; ClearForm(); }
    [RelayCommand] private void GoBack() => _mainViewModel.NavigateTo(new DashboardViewModel(_mainViewModel, new AuthService()));
    private void ClearForm() { FormVehicleId = 0; FormMonth = DateTime.Now.Month; FormYear = DateTime.Now.Year; FormPlannedAmount = 0; FormActualAmount = 0; SelectedBudget = null; }
}
