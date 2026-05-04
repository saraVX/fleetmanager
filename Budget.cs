using System;
using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class Budget
{
    [Key]
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public double PlannedAmount { get; set; }
    public double ActualAmount { get; set; }
    
    public Vehicle? Vehicle { get; set; }
}
