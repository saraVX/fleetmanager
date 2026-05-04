using CommunityToolkit.Mvvm.ComponentModel;

namespace FleetManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentView;
    
    public MainWindowViewModel()
    {
        CurrentView = new LoginViewModel(this);
    }
    
    public void NavigateTo(ViewModelBase viewModel)
    {
        CurrentView = viewModel;
    }
}
