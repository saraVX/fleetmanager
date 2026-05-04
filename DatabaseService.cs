using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FleetManager.Data;
using FleetManager.Models;

namespace FleetManager.Services;

public class DatabaseService
{
    private readonly ApplicationDbContext _context;
    
    public DatabaseService()
    {
        _context = new ApplicationDbContext();
        _context.Database.EnsureCreated();
    }
    
    // Users
    public async Task<List<User>> GetUsersAsync() => await _context.Users.ToListAsync();
    public async Task<User?> GetUserByUsernameAsync(string username) => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    public async Task<bool> AddUserAsync(User user) { _context.Users.Add(user); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> UpdateUserAsync(User user) { _context.Users.Update(user); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> DeleteUserAsync(int id) { var u = await _context.Users.FindAsync(id); if (u != null && u.Username != "admin") { _context.Users.Remove(u); return await _context.SaveChangesAsync() > 0; } return false; }
    
    // Vehicles
    public async Task<List<Vehicle>> GetVehiclesAsync() => await _context.Vehicles.ToListAsync();
    public async Task<Vehicle?> GetVehicleByIdAsync(int id) => await _context.Vehicles.FindAsync(id);
    public async Task<bool> AddVehicleAsync(Vehicle v) { _context.Vehicles.Add(v); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> UpdateVehicleAsync(Vehicle v) { _context.Vehicles.Update(v); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> DeleteVehicleAsync(int id) { var v = await _context.Vehicles.FindAsync(id); if (v != null) { _context.Vehicles.Remove(v); return await _context.SaveChangesAsync() > 0; } return false; }
    
    // Maintenances
    public async Task<List<Maintenance>> GetMaintenancesAsync() => await _context.Maintenances.Include(m => m.Vehicle).ToListAsync();
    public async Task<bool> AddMaintenanceAsync(Maintenance m) { _context.Maintenances.Add(m); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> UpdateMaintenanceAsync(Maintenance m) { _context.Maintenances.Update(m); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> DeleteMaintenanceAsync(int id) { var m = await _context.Maintenances.FindAsync(id); if (m != null) { _context.Maintenances.Remove(m); return await _context.SaveChangesAsync() > 0; } return false; }
    
    // Fuel Records
    public async Task<List<FuelRecord>> GetFuelRecordsAsync() => await _context.FuelRecords.Include(f => f.Vehicle).ToListAsync();
    public async Task<bool> AddFuelRecordAsync(FuelRecord f) { _context.FuelRecords.Add(f); return await _context.SaveChangesAsync() > 0; }
    
    // Insurance
    public async Task<List<Insurance>> GetInsurancesAsync() => await _context.Set<Insurance>().Include(i => i.Vehicle).ToListAsync();
    public async Task<bool> AddInsuranceAsync(Insurance i) { _context.Set<Insurance>().Add(i); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> UpdateInsuranceAsync(Insurance i) { _context.Set<Insurance>().Update(i); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> DeleteInsuranceAsync(int id) { var item = await _context.Set<Insurance>().FindAsync(id); if (item != null) { _context.Set<Insurance>().Remove(item); return await _context.SaveChangesAsync() > 0; } return false; }
    
    // Drivers
    public async Task<List<Driver>> GetDriversAsync() => await _context.Set<Driver>().ToListAsync();
    public async Task<bool> AddDriverAsync(Driver d) { _context.Set<Driver>().Add(d); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> UpdateDriverAsync(Driver d) { _context.Set<Driver>().Update(d); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> DeleteDriverAsync(int id) { var item = await _context.Set<Driver>().FindAsync(id); if (item != null) { _context.Set<Driver>().Remove(item); return await _context.SaveChangesAsync() > 0; } return false; }
    
    // Maintenance Schedule
    public async Task<List<MaintenanceSchedule>> GetMaintenanceSchedulesAsync() => await _context.Set<MaintenanceSchedule>().Include(m => m.Vehicle).ToListAsync();
    public async Task<bool> AddMaintenanceScheduleAsync(MaintenanceSchedule s) { _context.Set<MaintenanceSchedule>().Add(s); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> UpdateMaintenanceScheduleAsync(MaintenanceSchedule s) { _context.Set<MaintenanceSchedule>().Update(s); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> DeleteMaintenanceScheduleAsync(int id) { var item = await _context.Set<MaintenanceSchedule>().FindAsync(id); if (item != null) { _context.Set<MaintenanceSchedule>().Remove(item); return await _context.SaveChangesAsync() > 0; } return false; }
    
    // Budget
    public async Task<List<Budget>> GetBudgetsAsync() => await _context.Set<Budget>().Include(b => b.Vehicle).ToListAsync();
    public async Task<bool> AddBudgetAsync(Budget b) { _context.Set<Budget>().Add(b); return await _context.SaveChangesAsync() > 0; }
    
    // Statistics
    public async Task<int> GetTotalVehiclesAsync() => await _context.Vehicles.CountAsync();
    public async Task<int> GetVehiclesInMaintenanceAsync() => await _context.Vehicles.CountAsync(v => v.Status == "En maintenance");
    public async Task<int> GetAvailableVehiclesAsync() => await _context.Vehicles.CountAsync(v => v.Status == "Disponible");
    public async Task<double> GetAverageFuelConsumptionAsync()
    {
        var v = await _context.Vehicles.ToListAsync();
        return v.Any() ? v.Average(x => x.FuelConsumption) : 0;
    }
    public async Task<int> GetTotalMaintenanceCostAsync() => await _context.Maintenances.SumAsync(m => m.Cost);
    public async Task<int> GetTotalMileageAsync()
    {
        var v = await _context.Vehicles.ToListAsync();
        return v.Any() ? v.Sum(x => x.Mileage) : 0;
    }
    public async Task<int> GetTotalFuelCostAsync() => (int)await _context.FuelRecords.SumAsync(f => f.Cost);
    public async Task<List<Insurance>> GetExpiringInsurancesAsync(int days = 30)
    {
        var limit = DateTime.Now.AddDays(days);
        return await _context.Set<Insurance>().Where(i => i.EndDate <= limit).Include(i => i.Vehicle).ToListAsync();
    }
}
