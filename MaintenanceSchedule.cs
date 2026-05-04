using System;
using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class MaintenanceSchedule
{
    [Key]
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public int EstimatedCost { get; set; }
    public bool IsDone { get; set; }
    
    public Vehicle? Vehicle { get; set; }
}
