using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Models;
using FleetManager.Services;

namespace FleetManager.ViewModels;

public partial class SchedulesViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly DatabaseService _dbService;
    
    [ObservableProperty]
    private ObservableCollection<MaintenanceSchedule> _schedules = new();
    
    [ObservableProperty]
    private MaintenanceSchedule? _selectedSchedule;
    
    [ObservableProperty]
    private bool _isEditing;
    
    [ObservableProperty]
    private int _formVehicleId;
    
    [ObservableProperty]
    private string _formTitle = string.Empty;
    
    [ObservableProperty]
    private DateTime _formScheduledDate = DateTime.Now;
    
    [ObservableProperty]
    private string _formType = string.Empty;
    
    [ObservableProperty]
    private int _formEstimatedCost;
    
    [ObservableProperty]
    private int _completedCount;
    
    [ObservableProperty]
    private int _pendingCount;
    
    [ObservableProperty]
    private int _urgentCount;
    
    public SchedulesViewModel(MainWindowViewModel mainViewModel, DatabaseService dbService)
    {
        _mainViewModel = mainViewModel;
        _dbService = dbService;
        LoadData();
    }
    
    private async void LoadData()
    {
        var list = await _dbService.GetMaintenanceSchedulesAsync();
        Schedules = new ObservableCollection<MaintenanceSchedule>(list);
        UpdateStats();
    }
    
    private void UpdateStats()
    {
        CompletedCount = Schedules.Count(s => s.IsDone);
        PendingCount = Schedules.Count(s => !s.IsDone);
        UrgentCount = Schedules.Count(s => !s.IsDone && s.ScheduledDate <= DateTime.Now.AddDays(7));
    }
    
    [RelayCommand]
    private void NewSchedule()
    {
        IsEditing = true;
        ClearForm();
        SelectedSchedule = null;
    }
    
    [RelayCommand]
    private void EditSchedule()
    {
        if (SelectedSchedule == null) return;
        IsEditing = true;
        FormVehicleId = SelectedSchedule.VehicleId;
        FormTitle = SelectedSchedule.Title;
        FormScheduledDate = SelectedSchedule.ScheduledDate;
        FormType = SelectedSchedule.Type;
        FormEstimatedCost = SelectedSchedule.EstimatedCost;
    }
    
    [RelayCommand]
    private async Task SaveScheduleAsync()
    {
        if (SelectedSchedule != null && IsEditing)
        {
            SelectedSchedule.VehicleId = FormVehicleId;
            SelectedSchedule.Title = FormTitle;
            SelectedSchedule.ScheduledDate = FormScheduledDate;
            SelectedSchedule.Type = FormType;
            SelectedSchedule.EstimatedCost = FormEstimatedCost;
            await _dbService.UpdateMaintenanceScheduleAsync(SelectedSchedule);
        }
        else if (IsEditing)
        {
            var newSchedule = new MaintenanceSchedule
            {
                VehicleId = FormVehicleId,
                Title = FormTitle,
                ScheduledDate = FormScheduledDate,
                Type = FormType,
                EstimatedCost = FormEstimatedCost,
                IsDone = false
            };
            await _dbService.AddMaintenanceScheduleAsync(newSchedule);
        }
        
        IsEditing = false;
        LoadData();
        ClearForm();
    }
    
    [RelayCommand]
    private async Task DeleteScheduleAsync()
    {
        if (SelectedSchedule == null) return;
        await _dbService.DeleteMaintenanceScheduleAsync(SelectedSchedule.Id);
        LoadData();
        SelectedSchedule = null;
    }
    
    [RelayCommand]
    private async Task ToggleStatusAsync(MaintenanceSchedule schedule)
    {
        if (schedule != null)
        {
            schedule.IsDone = !schedule.IsDone;
            await _dbService.UpdateMaintenanceScheduleAsync(schedule);
            LoadData();
        }
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
        FormTitle = string.Empty;
        FormScheduledDate = DateTime.Now;
        FormType = string.Empty;
        FormEstimatedCost = 0;
        SelectedSchedule = null;
    }
}
