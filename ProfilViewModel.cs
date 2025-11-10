using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Models;
using FleetManager.Services;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;

namespace FleetManager.ViewModels;

public partial class ProfilViewModel : ObservableObject
{
    private readonly ServiceBaseDeDonnees _serviceBDD = new();

    [ObservableProperty]
    private string _nomUtilisateur = "";

    [ObservableProperty]
    private string _role = "";

    [ObservableProperty]
    private string _email = "";

    [ObservableProperty]
    private string _telephone = "";

    [ObservableProperty]
    private string _dateCreation = "";

    [ObservableProperty]
    private string _derniereConnexion = "";

    [ObservableProperty]
    private string _messageStatut = "";

    public ProfilViewModel()
    {
        ChargerProfil();
    }

    private async void ChargerProfil()
    {
        var utilisateur = await _serviceBDD.ObtenirProfilCourant();
        if (utilisateur != null)
        {
            NomUtilisateur = utilisateur.NomUtilisateur;
            Role = utilisateur.Role;
            Email = utilisateur.Email ?? "";
            Telephone = utilisateur.Telephone ?? "";
            DateCreation = utilisateur.DateCreation.ToString("dd/MM/yyyy HH:mm");
            DerniereConnexion = utilisateur.DerniereConnexion.ToString("dd/MM/yyyy HH:mm");
            MessageStatut = "Profil chargé";
        }
        else
        {
            MessageStatut = "Erreur: Aucun utilisateur connecté";
        }
    }

    [RelayCommand]
    private async Task Enregistrer()
    {
        if (string.IsNullOrWhiteSpace(NomUtilisateur))
        {
            MessageStatut = "Le nom d'utilisateur ne peut pas être vide";
            return;
        }

        if (NomUtilisateur.Length < 3)
        {
            MessageStatut = "Le nom d'utilisateur doit faire au moins 3 caractères";
            return;
        }

        MessageStatut = "Mise à jour en cours...";
        
        var succes = await _serviceBDD.MettreAJourProfil(NomUtilisateur, Email, Telephone);
        
        if (succes)
        {
            MessageStatut = "Profil mis à jour avec succès!";
        }
        else
        {
            MessageStatut = "Erreur : ce nom d'utilisateur est déjà pris";
        }
    }

    [RelayCommand]
    private void Fermer()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        desktop?.MainWindow?.Close();
    }
}
