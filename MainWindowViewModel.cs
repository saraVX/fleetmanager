using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FleetManager.Models;
using FleetManager.Services;
using FleetManager.Views;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace FleetManager.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ServiceBaseDeDonnees _serviceBDD = new();

    [ObservableProperty]
    private ObservableCollection<Vehicule> _vehicules = new();

    [ObservableProperty]
    private string _messageStatut = "Prêt";

    [ObservableProperty]
    private Vehicule? _vehiculeSelectionne;

    [ObservableProperty]
    private string _nouvelleMarque = "";

    [ObservableProperty]
    private string _nouveauModele = "";

    [ObservableProperty]
    private string _nouvelleImmatriculation = "";

    [ObservableProperty]
    private string _typeCarburantSelectionne = "essence";

    public MainWindowViewModel()
    {
        ChargerVehicules();
    }

    private async void ChargerVehicules()
    {
        MessageStatut = "Chargement des véhicules...";
        var vehicules = await _serviceBDD.ObtenirVehicules();
        Vehicules.Clear();
        foreach (var vehicule in vehicules)
        {
            Vehicules.Add(vehicule);
        }
        MessageStatut = $"Chargé: {Vehicules.Count} véhicules";
    }

    [RelayCommand]
    private async Task AjouterVehicule()
    {
        if (string.IsNullOrWhiteSpace(NouvelleMarque) || 
            string.IsNullOrWhiteSpace(NouveauModele) || 
            string.IsNullOrWhiteSpace(NouvelleImmatriculation))
        {
            MessageStatut = "Veuillez remplir tous les champs";
            return;
        }

        MessageStatut = "Ajout en cours...";

        var nouveauVehicule = new Vehicule
        {
            Marque = NouvelleMarque,
            Modele = NouveauModele,
            Immatriculation = NouvelleImmatriculation,
            TypeCarburant = TypeCarburantSelectionne
        };

        var succes = await _serviceBDD.AjouterVehicule(nouveauVehicule);
        
        if (succes)
        {
            await ChargerVehiculesAsync();
            NouvelleMarque = "";
            NouveauModele = "";
            NouvelleImmatriculation = "";
            MessageStatut = "Véhicule ajouté avec succès!";
        }
        else
        {
            MessageStatut = "Erreur lors de l'ajout";
        }
    }

    [RelayCommand]
    private async Task ModifierVehicule()
    {
        if (VehiculeSelectionne == null)
        {
            MessageStatut = "Veuillez sélectionner un véhicule";
            return;
        }

        MessageStatut = "Modification en cours...";

        var succes = await _serviceBDD.ModifierVehicule(VehiculeSelectionne);
        
        if (succes)
        {
            MessageStatut = "Véhicule modifié avec succès!";
            await ChargerVehiculesAsync();
        }
        else
        {
            MessageStatut = "Erreur lors de la modification";
        }
    }

    [RelayCommand]
    private async Task SupprimerVehicule()
    {
        if (VehiculeSelectionne == null)
        {
            MessageStatut = "Veuillez sélectionner un véhicule";
            return;
        }

        MessageStatut = "Suppression en cours...";

        var succes = await _serviceBDD.SupprimerVehicule(VehiculeSelectionne.Id);
        
        if (succes)
        {
            MessageStatut = "Véhicule supprimé avec succès!";
            await ChargerVehiculesAsync();
        }
        else
        {
            MessageStatut = "Erreur lors de la suppression";
        }
    }

    [RelayCommand]
    private void OuvrirProfil()
    {
        var profilWindow = new ProfilWindow
        {
            DataContext = new ProfilViewModel()
        };
        profilWindow.Show();
    }

    [RelayCommand]
    private void Deconnexion()
    {
        _serviceBDD.DeconnecterUtilisateur();
        
        var loginWindow = new LoginWindow
        {
            DataContext = new LoginViewModel()
        };
        loginWindow.Show();

        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (desktop?.MainWindow != null)
        {
            desktop.MainWindow.Close();
            desktop.MainWindow = loginWindow;
        }
    }

    private async Task ChargerVehiculesAsync()
    {
        var vehicules = await _serviceBDD.ObtenirVehicules();
        Vehicules.Clear();
        foreach (var vehicule in vehicules)
        {
            Vehicules.Add(vehicule);
        }
    }
}
