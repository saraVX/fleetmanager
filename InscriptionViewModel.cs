using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Services;
using FleetManager.Views;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;

namespace FleetManager.ViewModels;

public partial class InscriptionViewModel : ObservableObject
{
    private readonly ServiceBaseDeDonnees _serviceBDD = new();

    [ObservableProperty]
    private string _nomUtilisateur = "";

    [ObservableProperty]
    private string _motDePasse = "";

    [ObservableProperty]
    private string _confirmationMotDePasse = "";

    [ObservableProperty]
    private string _email = "";

    [ObservableProperty]
    private string _telephone = "";

    [ObservableProperty]
    private string _messageErreur = "";

    [RelayCommand]
    private async Task Inscription()
    {
        MessageErreur = "Traitement en cours...";

        // Validation
        if (string.IsNullOrWhiteSpace(NomUtilisateur) || 
            string.IsNullOrWhiteSpace(MotDePasse) || 
            string.IsNullOrWhiteSpace(ConfirmationMotDePasse))
        {
            MessageErreur = "Veuillez remplir tous les champs obligatoires (*)";
            return;
        }

        if (NomUtilisateur.Length < 3)
        {
            MessageErreur = "Le nom d'utilisateur doit faire au moins 3 caractères";
            return;
        }

        if (MotDePasse.Length < 4)
        {
            MessageErreur = "Le mot de passe doit faire au moins 4 caractères";
            return;
        }

        if (MotDePasse != ConfirmationMotDePasse)
        {
            MessageErreur = "Les mots de passe ne correspondent pas";
            return;
        }

        // Inscription
        var succes = await _serviceBDD.InscrireUtilisateur(NomUtilisateur, MotDePasse, Email, Telephone);
        
        if (succes)
        {
            MessageErreur = "Inscription réussie! Redirection...";
            await Task.Delay(1500);
            
            // Ouvrir la fenêtre principale directement
            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
            mainWindow.Show();
            
            // Fermer la fenêtre d'inscription
            var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (desktop?.MainWindow != null)
            {
                desktop.MainWindow.Close();
                desktop.MainWindow = mainWindow;
            }
        }
        else
        {
            MessageErreur = "Erreur : ce nom d'utilisateur est déjà pris";
        }
    }

    [RelayCommand]
    private void Annuler()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        desktop?.MainWindow?.Close();
    }
}
