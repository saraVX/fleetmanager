using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FleetManager.Models;

namespace FleetManager.Services;

public class ServiceBaseDeDonnees
{
    private List<Vehicule> _vehiculesEnMemoire = new()
    {
        new Vehicule { Id = 1, Marque = "Peugeot", Modele = "308", Immatriculation = "AB-123-CD", TypeCarburant = "diesel" },
        new Vehicule { Id = 2, Marque = "Renault", Modele = "Clio", Immatriculation = "EF-456-GH", TypeCarburant = "essence" },
        new Vehicule { Id = 3, Marque = "Citroën", Modele = "C4", Immatriculation = "IJ-789-KL", TypeCarburant = "essence" }
    };

    private List<Utilisateur> _utilisateursEnMemoire = new()
    {
        new Utilisateur { Id = 1, NomUtilisateur = "admin", MotDePasseHash = "0192023a7bbd73250516f069df18b500", Role = "Administrateur", Email = "admin@fleet.com", Telephone = "0123456789" }
    };

    private int _prochainIdVehicule = 4;
    private int _prochainIdUtilisateur = 2;

    public Utilisateur? UtilisateurCourant { get; private set; }

    // AUTHENTIFICATION
    public Task<bool> AuthentifierUtilisateur(string nomUtilisateur, string motDePasse)
    {
        var hash = CalculerMD5(motDePasse);
        var utilisateur = _utilisateursEnMemoire.Find(u => u.NomUtilisateur == nomUtilisateur && u.MotDePasseHash == hash);
        
        if (utilisateur != null)
        {
            UtilisateurCourant = utilisateur;
            utilisateur.DerniereConnexion = DateTime.Now;
            Console.WriteLine($"✅ Connexion réussie: {nomUtilisateur}");
            return Task.FromResult(true);
        }
        
        Console.WriteLine($"❌ Échec connexion: {nomUtilisateur}");
        return Task.FromResult(false);
    }

    // INSCRIPTION
    public Task<bool> InscrireUtilisateur(string nomUtilisateur, string motDePasse, string email, string telephone)
    {
        // Vérifier si l'utilisateur existe déjà
        if (_utilisateursEnMemoire.Exists(u => u.NomUtilisateur == nomUtilisateur))
        {
            Console.WriteLine($"❌ Utilisateur déjà existant: {nomUtilisateur}");
            return Task.FromResult(false);
        }

        var nouvelUtilisateur = new Utilisateur
        {
            Id = _prochainIdUtilisateur++,
            NomUtilisateur = nomUtilisateur,
            MotDePasseHash = CalculerMD5(motDePasse),
            Role = "Utilisateur",
            Email = email,
            Telephone = telephone,
            DateCreation = DateTime.Now,
            DerniereConnexion = DateTime.Now
        };

        _utilisateursEnMemoire.Add(nouvelUtilisateur);
        UtilisateurCourant = nouvelUtilisateur;
        
        Console.WriteLine($"✅ Inscription réussie: {nomUtilisateur}");
        return Task.FromResult(true);
    }

    // METTRE À JOUR PROFIL
    public Task<bool> MettreAJourProfil(string nouveauNom, string email, string telephone)
    {
        if (UtilisateurCourant == null)
        {
            Console.WriteLine("❌ Aucun utilisateur connecté");
            return Task.FromResult(false);
        }

        // Vérifier si le nouveau nom n'est pas déjà pris
        if (nouveauNom != UtilisateurCourant.NomUtilisateur && 
            _utilisateursEnMemoire.Exists(u => u.NomUtilisateur == nouveauNom))
        {
            Console.WriteLine($"❌ Nom d'utilisateur déjà pris: {nouveauNom}");
            return Task.FromResult(false);
        }

        // Mettre à jour les informations
        UtilisateurCourant.NomUtilisateur = nouveauNom;
        UtilisateurCourant.Email = email;
        UtilisateurCourant.Telephone = telephone;

        // Mettre à jour dans la liste
        var utilisateurEnBase = _utilisateursEnMemoire.Find(u => u.Id == UtilisateurCourant.Id);
        if (utilisateurEnBase != null)
        {
            utilisateurEnBase.NomUtilisateur = nouveauNom;
            utilisateurEnBase.Email = email;
            utilisateurEnBase.Telephone = telephone;
        }

        Console.WriteLine($"✅ Profil mis à jour: {nouveauNom}");
        return Task.FromResult(true);
    }

    // OBTENIR PROFIL COURANT
    public Task<Utilisateur?> ObtenirProfilCourant()
    {
        return Task.FromResult(UtilisateurCourant);
    }

    // DÉCONNEXION
    public void DeconnecterUtilisateur()
    {
        UtilisateurCourant = null;
        Console.WriteLine("✅ Utilisateur déconnecté");
    }

    private string CalculerMD5(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLower();
    }

    // CRUD VÉHICULES
    public Task<List<Vehicule>> ObtenirVehicules()
    {
        Console.WriteLine($"📋 Chargement de {_vehiculesEnMemoire.Count} véhicules");
        return Task.FromResult(new List<Vehicule>(_vehiculesEnMemoire));
    }

    public Task<bool> AjouterVehicule(Vehicule vehicule)
    {
        try
        {
            vehicule.Id = _prochainIdVehicule++;
            _vehiculesEnMemoire.Add(vehicule);
            Console.WriteLine($"✅ Véhicule ajouté: {vehicule.Marque} {vehicule.Modele} - {vehicule.Immatriculation}");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur ajout véhicule: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    public Task<bool> ModifierVehicule(Vehicule vehiculeModifie)
    {
        try
        {
            var vehiculeExistant = _vehiculesEnMemoire.Find(v => v.Id == vehiculeModifie.Id);
            if (vehiculeExistant != null)
            {
                vehiculeExistant.Marque = vehiculeModifie.Marque;
                vehiculeExistant.Modele = vehiculeModifie.Modele;
                vehiculeExistant.Immatriculation = vehiculeModifie.Immatriculation;
                vehiculeExistant.TypeCarburant = vehiculeModifie.TypeCarburant;
                Console.WriteLine($"✅ Véhicule modifié: {vehiculeModifie.Marque} {vehiculeModifie.Modele}");
                return Task.FromResult(true);
            }
            Console.WriteLine($"❌ Véhicule non trouvé: ID {vehiculeModifie.Id}");
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur modification véhicule: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    public Task<bool> SupprimerVehicule(int id)
    {
        try
        {
            var vehiculeASupprimer = _vehiculesEnMemoire.Find(v => v.Id == id);
            if (vehiculeASupprimer != null)
            {
                _vehiculesEnMemoire.Remove(vehiculeASupprimer);
                Console.WriteLine($"✅ Véhicule supprimé: {vehiculeASupprimer.Marque} {vehiculeASupprimer.Modele}");
                return Task.FromResult(true);
            }
            Console.WriteLine($"❌ Véhicule non trouvé: ID {id}");
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur suppression véhicule: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    public Task<Vehicule?> ObtenirVehiculeParId(int id)
    {
        var vehicule = _vehiculesEnMemoire.Find(v => v.Id == id);
        return Task.FromResult(vehicule);
    }
}
