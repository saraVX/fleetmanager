using System;
using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class Insurance
{
    [Key]
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string Company { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double Cost { get; set; }
    
    public virtual Vehicle? Vehicle { get; set; }
}
