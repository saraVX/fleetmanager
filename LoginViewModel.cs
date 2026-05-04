using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Services;

namespace FleetManager.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly AuthService _authService;
    
    [ObservableProperty]
    private string _username = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    public LoginViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _authService = new AuthService();
    }
    
    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Veuillez remplir tous les champs";
            return;
        }
        
        var success = await _authService.LoginAsync(Username, Password);
        if (success)
        {
            ErrorMessage = string.Empty;
            var dashboard = new DashboardViewModel(_mainViewModel, _authService);
            _mainViewModel.NavigateTo(dashboard);
        }
        else
        {
            ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect";
        }
    }
}
