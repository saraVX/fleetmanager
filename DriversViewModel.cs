using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Models;
using FleetManager.Services;

namespace FleetManager.ViewModels;

public partial class DriversViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly DatabaseService _dbService;
    
    [ObservableProperty] private ObservableCollection<Driver> _drivers = new();
    [ObservableProperty] private Driver? _selectedDriver;
    [ObservableProperty] private bool _isEditing;
    [ObservableProperty] private string _formFirstName = string.Empty;
    [ObservableProperty] private string _formLastName = string.Empty;
    [ObservableProperty] private string _formLicenseNumber = string.Empty;
    [ObservableProperty] private DateTime _formLicenseExpiry = DateTime.Now.AddYears(1);
    [ObservableProperty] private string _formPhone = string.Empty;
    [ObservableProperty] private string _formEmail = string.Empty;
    
    public DriversViewModel(MainWindowViewModel mainViewModel, DatabaseService dbService)
    {
        _mainViewModel = mainViewModel;
        _dbService = dbService;
        LoadData();
    }
    
    private async void LoadData() => Drivers = new ObservableCollection<Driver>(await _dbService.GetDriversAsync());
    
    [RelayCommand] private void New() { IsEditing = true; ClearForm(); }
    [RelayCommand] private void Edit() { if (SelectedDriver != null) { IsEditing = true; FormFirstName = SelectedDriver.FirstName; FormLastName = SelectedDriver.LastName; FormLicenseNumber = SelectedDriver.LicenseNumber; FormLicenseExpiry = SelectedDriver.LicenseExpiry; FormPhone = SelectedDriver.Phone; FormEmail = SelectedDriver.Email; } }
    [RelayCommand] private async Task Save() { if (SelectedDriver != null && IsEditing) { SelectedDriver.FirstName = FormFirstName; SelectedDriver.LastName = FormLastName; SelectedDriver.LicenseNumber = FormLicenseNumber; SelectedDriver.LicenseExpiry = FormLicenseExpiry; SelectedDriver.Phone = FormPhone; SelectedDriver.Email = FormEmail; await _dbService.UpdateDriverAsync(SelectedDriver); } else if (IsEditing) { await _dbService.AddDriverAsync(new Driver { FirstName = FormFirstName, LastName = FormLastName, LicenseNumber = FormLicenseNumber, LicenseExpiry = FormLicenseExpiry, Phone = FormPhone, Email = FormEmail }); } IsEditing = false; LoadData(); ClearForm(); }
    [RelayCommand] private async Task Delete() { if (SelectedDriver != null) { await _dbService.DeleteDriverAsync(SelectedDriver.Id); LoadData(); } }
    [RelayCommand] private void Cancel() { IsEditing = false; ClearForm(); }
    [RelayCommand] private void GoBack() => _mainViewModel.NavigateTo(new DashboardViewModel(_mainViewModel, new AuthService()));
    private void ClearForm() { FormFirstName = string.Empty; FormLastName = string.Empty; FormLicenseNumber = string.Empty; FormLicenseExpiry = DateTime.Now.AddYears(1); FormPhone = string.Empty; FormEmail = string.Empty; SelectedDriver = null; }
}
