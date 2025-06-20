using Supabase.Gotrue;
using SupabaseService.Models;

namespace SupabaseService.Repository;

public partial class SupabaseRepository
{
    public bool IsLoggedIn
    {
        get => _isInitialized && CloudDatabase?.SupabaseClient.Auth.CurrentSession != null;
    }
    
    public async Task Login(string email, string password)
    {
        await EnsureInitializedAsync();
        await CloudDatabase!.SupabaseClient.Auth.SignIn(email, password);
    }
    
    public async Task Register(string email, string password, string username)
    {
        await EnsureInitializedAsync();
        var signUpOptions = new SignUpOptions
        {
            Data = new Dictionary<string, object>
            {
                { "username", username }
            }
        };
        await CloudDatabase!.SupabaseClient.Auth.SignUp(email, password, signUpOptions);
    }
    
    public async Task Logout()
    {
        await EnsureInitializedAsync();
        await CloudDatabase!.SupabaseClient.Auth.SignOut();
    }
    
    public string? GetCurrentUserId()
    {
        return _isInitialized ? CloudDatabase?.SupabaseClient.Auth.CurrentUser?.Id : null;
    }
    
    public string? GetCurrentUserEmail()
    {
        return _isInitialized ? CloudDatabase?.SupabaseClient.Auth.CurrentUser?.Email : null;
    }
    
    public UserDbModel? GetCurrentUser()
    {
        return null;
    }
}