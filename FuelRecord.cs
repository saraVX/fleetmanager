using System;
using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class FuelRecord
{
    [Key]
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime RefuelDate { get; set; }
    public double Liters { get; set; }
    public double Cost { get; set; }
    public int Mileage { get; set; }
    
    public virtual Vehicle? Vehicle { get; set; }
}
