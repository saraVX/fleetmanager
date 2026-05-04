using System;
using System.Threading.Tasks;
using FleetManager.Models;

namespace FleetManager.Services;

public class AuthService
{
    private readonly DatabaseService _dbService;
    
    public AuthService()
    {
        _dbService = new DatabaseService();
    }
    
    public User? CurrentUser => UserSession.Instance.CurrentUser;
    public bool IsAdmin => UserSession.Instance.IsAdmin;
    
    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = await _dbService.GetUserByUsernameAsync(username);
        if (user == null) return false;
        
        if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            UserSession.Instance.SetUser(user);
            return true;
        }
        return false;
    }
    
    public async Task<bool> UpdateProfileAsync(string username, string email, string? newPassword = null)
    {
        if (CurrentUser == null) return false;
        
        CurrentUser.Username = username;
        CurrentUser.Email = email;
        if (!string.IsNullOrEmpty(newPassword))
        {
            CurrentUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        }
        
        return await _dbService.UpdateUserAsync(CurrentUser);
    }
    
    public void Logout()
    {
        UserSession.Instance.ClearUser();
    }
}
