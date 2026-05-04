using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using FleetManager.Models;

namespace FleetManager.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Maintenance> Maintenances { get; set; }
    public DbSet<FuelRecord> FuelRecords { get; set; }
    public DbSet<Insurance> Insurances { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=fleetmanager.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Maintenance>().HasOne(m => m.Vehicle).WithMany().HasForeignKey(m => m.VehicleId);
        modelBuilder.Entity<FuelRecord>().HasOne(f => f.Vehicle).WithMany().HasForeignKey(f => f.VehicleId);
        modelBuilder.Entity<Insurance>().HasOne(i => i.Vehicle).WithMany().HasForeignKey(i => i.VehicleId);
        modelBuilder.Entity<MaintenanceSchedule>().HasOne(m => m.Vehicle).WithMany().HasForeignKey(m => m.VehicleId);
        modelBuilder.Entity<Budget>().HasOne(b => b.Vehicle).WithMany().HasForeignKey(b => b.VehicleId);
        
        // Admin
        modelBuilder.Entity<User>().HasData(new User { Id = 1, Username = "admin", Email = "admin@fleetmanager.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "Admin", CreatedAt = DateTime.Now });
        modelBuilder.Entity<User>().HasData(new User { Id = 2, Username = "agent", Email = "agent@fleetmanager.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("agent123"), Role = "Agent", CreatedAt = DateTime.Now });
        
        // 4 Véhicules
        modelBuilder.Entity<Vehicle>().HasData(
            new Vehicle { Id = 1, LicensePlate = "AB-123-CD", Brand = "Renault", Model = "Clio", Year = 2022, Color = "Rouge", Mileage = 12500, Status = "Disponible", FuelConsumption = 5.8, CreatedAt = DateTime.Now },
            new Vehicle { Id = 2, LicensePlate = "EF-456-GH", Brand = "Peugeot", Model = "208", Year = 2023, Color = "Noir", Mileage = 8900, Status = "Disponible", FuelConsumption = 5.5, CreatedAt = DateTime.Now },
            new Vehicle { Id = 3, LicensePlate = "IJ-789-KL", Brand = "Citroen", Model = "C3", Year = 2021, Color = "Bleu", Mileage = 34200, Status = "En maintenance", FuelConsumption = 6.2, CreatedAt = DateTime.Now },
            new Vehicle { Id = 4, LicensePlate = "MN-012-OP", Brand = "Toyota", Model = "Yaris", Year = 2022, Color = "Blanc", Mileage = 18700, Status = "Disponible", FuelConsumption = 4.8, CreatedAt = DateTime.Now }
        );
        
        // Entretiens pour chaque véhicule
        modelBuilder.Entity<Maintenance>().HasData(
            new Maintenance { Id = 1, VehicleId = 1, MaintenanceDate = DateTime.Now.AddMonths(-3), MaintenanceType = "Vidange", Description = "Changement d'huile et filtres", Cost = 150, MileageAtMaintenance = 11000, Mechanic = "Garage Central" },
            new Maintenance { Id = 2, VehicleId = 2, MaintenanceDate = DateTime.Now.AddMonths(-2), MaintenanceType = "Freins", Description = "Remplacement plaquettes avant", Cost = 200, MileageAtMaintenance = 8000, Mechanic = "Garage Nord" },
            new Maintenance { Id = 3, VehicleId = 3, MaintenanceDate = DateTime.Now.AddMonths(-5), MaintenanceType = "Révision", Description = "Révision complète + vidange", Cost = 350, MileageAtMaintenance = 33000, Mechanic = "Garage Sud" },
            new Maintenance { Id = 4, VehicleId = 4, MaintenanceDate = DateTime.Now.AddMonths(-1), MaintenanceType = "Pneus", Description = "Changement des 4 pneus", Cost = 400, MileageAtMaintenance = 18000, Mechanic = "Garage Est" }
        );
        
        // Assurances
        modelBuilder.Entity<Insurance>().HasData(
            new Insurance { Id = 1, VehicleId = 1, Company = "AXA", PolicyNumber = "AX123456", StartDate = DateTime.Now.AddYears(-1), EndDate = DateTime.Now.AddMonths(2), Cost = 450 },
            new Insurance { Id = 2, VehicleId = 2, Company = "MAIF", PolicyNumber = "MA789012", StartDate = DateTime.Now.AddMonths(-8), EndDate = DateTime.Now.AddMonths(4), Cost = 420 },
            new Insurance { Id = 3, VehicleId = 3, Company = "Groupama", PolicyNumber = "GR345678", StartDate = DateTime.Now.AddMonths(-10), EndDate = DateTime.Now.AddMonths(1), Cost = 500 },
            new Insurance { Id = 4, VehicleId = 4, Company = "Generali", PolicyNumber = "GE901234", StartDate = DateTime.Now.AddMonths(-6), EndDate = DateTime.Now.AddMonths(6), Cost = 380 }
        );
        
        // Planification entretien préventif
        modelBuilder.Entity<MaintenanceSchedule>().HasData(
            new MaintenanceSchedule { Id = 1, VehicleId = 1, Title = "Vidange + filtres", ScheduledDate = DateTime.Now.AddDays(15), Type = "Préventif", EstimatedCost = 150, IsDone = false },
            new MaintenanceSchedule { Id = 2, VehicleId = 2, Title = "Contrôle technique", ScheduledDate = DateTime.Now.AddDays(25), Type = "Obligatoire", EstimatedCost = 80, IsDone = false },
            new MaintenanceSchedule { Id = 3, VehicleId = 3, Title = "Changement pneus", ScheduledDate = DateTime.Now.AddDays(10), Type = "Préventif", EstimatedCost = 400, IsDone = false },
            new MaintenanceSchedule { Id = 4, VehicleId = 4, Title = "Révision annuelle", ScheduledDate = DateTime.Now.AddDays(30), Type = "Préventif", EstimatedCost = 250, IsDone = false }
        );
    }
}
