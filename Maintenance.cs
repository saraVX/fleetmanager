using System;
using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class Maintenance
{
    [Key]
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public string MaintenanceType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Cost { get; set; }
    public int MileageAtMaintenance { get; set; }
    public string Mechanic { get; set; } = string.Empty;
    
    public virtual Vehicle? Vehicle { get; set; }
}
