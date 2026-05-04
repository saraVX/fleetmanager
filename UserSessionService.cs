using FleetManager.Models;

namespace FleetManager.Services;

public class UserSessionService
{
    public User? CurrentUser { get; set; }
    
    public bool IsAdmin => CurrentUser?.Role == "Admin";
    
    public void SetUser(User user)
    {
        CurrentUser = user;
    }
    
    public void ClearUser()
    {
        CurrentUser = null;
    }
}
