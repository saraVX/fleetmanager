using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Services;
using FleetManager.Views;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;

namespace FleetManager.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ServiceBaseDeDonnees _serviceBDD = new();

    [ObservableProperty]
    private string _nomUtilisateur = "";

    [ObservableProperty]
    private string _motDePasse = "";

    [ObservableProperty]
    private string _messageErreur = "";

    [RelayCommand]
    private async Task Connexion()
    {
        if (string.IsNullOrWhiteSpace(NomUtilisateur) || string.IsNullOrWhiteSpace(MotDePasse))
        {
            MessageErreur = "❌ Veuillez remplir tous les champs";
            return;
        }

        MessageErreur = "Connexion en cours...";

        var authentifie = await _serviceBDD.AuthentifierUtilisateur(NomUtilisateur, MotDePasse);
        
        if (authentifie)
        {
            MessageErreur = "✅ Connexion réussie!";
            await Task.Delay(500);
            
            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
            mainWindow.Show();
            
            var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (desktop != null)
            {
                desktop.MainWindow?.Close();
                desktop.MainWindow = mainWindow;
            }
        }
        else
        {
            MessageErreur = "❌ Nom d'utilisateur ou mot de passe incorrect";
        }
    }

    [RelayCommand]
    private void OuvrirInscription()
    {
        var inscriptionWindow = new InscriptionWindow();
        inscriptionWindow.Show();
    }
}
