using System;
using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class Vehicle
{
    [Key]
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public string Status { get; set; } = "Disponible";
    public double FuelConsumption { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
