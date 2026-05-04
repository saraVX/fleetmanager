using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Models;
using FleetManager.Services;

namespace FleetManager.ViewModels;

public partial class InsurancesViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly DatabaseService _dbService;
    
    [ObservableProperty] private ObservableCollection<Insurance> _insurances = new();
    [ObservableProperty] private Insurance? _selectedInsurance;
    [ObservableProperty] private bool _isEditing;
    [ObservableProperty] private int _formVehicleId;
    [ObservableProperty] private string _formCompany = string.Empty;
    [ObservableProperty] private string _formPolicyNumber = string.Empty;
    [ObservableProperty] private DateTime _formStartDate = DateTime.Now;
    [ObservableProperty] private DateTime _formEndDate = DateTime.Now.AddYears(1);
    [ObservableProperty] private double _formCost;
    
    public InsurancesViewModel(MainWindowViewModel mainViewModel, DatabaseService dbService)
    {
        _mainViewModel = mainViewModel;
        _dbService = dbService;
        LoadData();
    }
    
    private async void LoadData() => Insurances = new ObservableCollection<Insurance>(await _dbService.GetInsurancesAsync());
    
    [RelayCommand] private void New() { IsEditing = true; ClearForm(); }
    [RelayCommand] private void Edit() { if (SelectedInsurance != null) { IsEditing = true; FormVehicleId = SelectedInsurance.VehicleId; FormCompany = SelectedInsurance.Company; FormPolicyNumber = SelectedInsurance.PolicyNumber; FormStartDate = SelectedInsurance.StartDate; FormEndDate = SelectedInsurance.EndDate; FormCost = SelectedInsurance.Cost; } }
    [RelayCommand] private async Task Save() { if (SelectedInsurance != null && IsEditing) { SelectedInsurance.VehicleId = FormVehicleId; SelectedInsurance.Company = FormCompany; SelectedInsurance.PolicyNumber = FormPolicyNumber; SelectedInsurance.StartDate = FormStartDate; SelectedInsurance.EndDate = FormEndDate; SelectedInsurance.Cost = FormCost; await _dbService.UpdateInsuranceAsync(SelectedInsurance); } else if (IsEditing) { await _dbService.AddInsuranceAsync(new Insurance { VehicleId = FormVehicleId, Company = FormCompany, PolicyNumber = FormPolicyNumber, StartDate = FormStartDate, EndDate = FormEndDate, Cost = FormCost }); } IsEditing = false; LoadData(); ClearForm(); }
    [RelayCommand] private async Task Delete() { if (SelectedInsurance != null) { await _dbService.DeleteInsuranceAsync(SelectedInsurance.Id); LoadData(); } }
    [RelayCommand] private void Cancel() { IsEditing = false; ClearForm(); }
    [RelayCommand] private void GoBack() => _mainViewModel.NavigateTo(new DashboardViewModel(_mainViewModel, new AuthService()));
    private void ClearForm() { FormVehicleId = 0; FormCompany = string.Empty; FormPolicyNumber = string.Empty; FormStartDate = DateTime.Now; FormEndDate = DateTime.Now.AddYears(1); FormCost = 0; SelectedInsurance = null; }
}
