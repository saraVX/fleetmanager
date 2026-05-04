using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Services;

namespace FleetManager.ViewModels;

public partial class ProfileViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly AuthService _authService;
    
    [ObservableProperty]
    private string _username = string.Empty;
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _newPassword = string.Empty;
    
    [ObservableProperty]
    private string _confirmPassword = string.Empty;
    
    [ObservableProperty]
    private string _message = string.Empty;
    
    [ObservableProperty]
    private bool _isSuccess;
    
    public ProfileViewModel(MainWindowViewModel mainViewModel, AuthService authService)
    {
        _mainViewModel = mainViewModel;
        _authService = authService;
        
        if (_authService.CurrentUser != null)
        {
            Username = _authService.CurrentUser.Username;
            Email = _authService.CurrentUser.Email;
        }
    }
    
    [RelayCommand]
    private async Task UpdateProfileAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email))
        {
            Message = "Veuillez remplir tous les champs";
            IsSuccess = false;
            return;
        }
        
        string? newPasswordHash = null;
        if (!string.IsNullOrWhiteSpace(NewPassword))
        {
            if (NewPassword != ConfirmPassword)
            {
                Message = "Les mots de passe ne correspondent pas";
                IsSuccess = false;
                return;
            }
            if (NewPassword.Length < 6)
            {
                Message = "Le mot de passe doit contenir au moins 6 caracteres";
                IsSuccess = false;
                return;
            }
            newPasswordHash = NewPassword;
        }
        
        var success = await _authService.UpdateProfileAsync(Username, Email, newPasswordHash);
        if (success)
        {
            Message = "Profil mis a jour avec succes !";
            IsSuccess = true;
            NewPassword = ConfirmPassword = string.Empty;
        }
        else
        {
            Message = "Erreur lors de la mise a jour";
            IsSuccess = false;
        }
    }
    
    [RelayCommand]
    private void GoBack()
    {
        var dashboard = new DashboardViewModel(_mainViewModel, _authService);
        _mainViewModel.NavigateTo(dashboard);
    }
}
