using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Models;
using FleetManager.Services;

namespace FleetManager.ViewModels;

public partial class UsersViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly DatabaseService _dbService;
    
    [ObservableProperty]
    private ObservableCollection<User> _users = new();
    
    [ObservableProperty]
    private User? _selectedUser;
    
    [ObservableProperty]
    private bool _isEditing;
    
    [ObservableProperty]
    private string _formUsername = string.Empty;
    
    [ObservableProperty]
    private string _formEmail = string.Empty;
    
    [ObservableProperty]
    private string _formPassword = string.Empty;
    
    [ObservableProperty]
    private string _formRole = "Agent";
    
    [ObservableProperty]
    private string _message = string.Empty;
    
    [ObservableProperty]
    private List<string> _roleList = new() { "Admin", "Agent" };
    
    public UsersViewModel(MainWindowViewModel mainViewModel, DatabaseService dbService)
    {
        _mainViewModel = mainViewModel;
        _dbService = dbService;
        LoadUsers();
    }
    
    private async void LoadUsers()
    {
        Users = new ObservableCollection<User>(await _dbService.GetUsersAsync());
    }
    
    [RelayCommand]
    private void NewUser()
    {
        IsEditing = true;
        ClearForm();
        Message = string.Empty;
    }
    
    [RelayCommand]
    private void EditUser()
    {
        if (SelectedUser == null)
        {
            Message = "Sélectionnez un utilisateur à modifier";
            return;
        }
        IsEditing = true;
        FormUsername = SelectedUser.Username;
        FormEmail = SelectedUser.Email;
        FormRole = SelectedUser.Role;
        FormPassword = string.Empty;
        Message = string.Empty;
    }
    
    [RelayCommand]
    private async Task SaveUserAsync()
    {
        if (string.IsNullOrWhiteSpace(FormUsername) || string.IsNullOrWhiteSpace(FormEmail))
        {
            Message = "Veuillez remplir tous les champs";
            return;
        }
        
        if (SelectedUser != null && IsEditing)
        {
            SelectedUser.Username = FormUsername;
            SelectedUser.Email = FormEmail;
            SelectedUser.Role = FormRole;
            if (!string.IsNullOrWhiteSpace(FormPassword))
            {
                SelectedUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(FormPassword);
            }
            await _dbService.UpdateUserAsync(SelectedUser);
            Message = "Utilisateur modifié avec succès";
        }
        else if (IsEditing)
        {
            if (string.IsNullOrWhiteSpace(FormPassword))
            {
                Message = "Veuillez entrer un mot de passe";
                return;
            }
            var newUser = new User
            {
                Username = FormUsername,
                Email = FormEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(FormPassword),
                Role = FormRole,
                CreatedAt = DateTime.Now
            };
            await _dbService.AddUserAsync(newUser);
            Message = "Utilisateur ajouté avec succès";
        }
        
        IsEditing = false;
        LoadUsers();
        ClearForm();
    }
    
    [RelayCommand]
    private async Task DeleteUserAsync()
    {
        if (SelectedUser == null)
        {
            Message = "Sélectionnez un utilisateur à supprimer";
            return;
        }
        if (SelectedUser.Username == "admin")
        {
            Message = "Impossible de supprimer l'utilisateur admin";
            return;
        }
        await _dbService.DeleteUserAsync(SelectedUser.Id);
        Message = "Utilisateur supprimé avec succès";
        LoadUsers();
        SelectedUser = null;
    }
    
    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
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
        FormUsername = string.Empty;
        FormEmail = string.Empty;
        FormPassword = string.Empty;
        FormRole = "Agent";
        SelectedUser = null;
    }
}
