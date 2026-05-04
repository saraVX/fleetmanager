using FleetManager.Models;

namespace FleetManager.Services;

public class UserSession
{
    private static UserSession? _instance;
    private static readonly object _lock = new object();
    
    public User? CurrentUser { get; set; }
    
    public static UserSession Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new UserSession();
                return _instance;
            }
        }
    }
    
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
