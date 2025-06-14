using Supabase.Gotrue;

namespace SupabaseService.Repository;

public partial class SupabaseRepository
{
    public bool IsLoggedIn
    {
        get => CloudDatabase?.SupabaseClient.Auth.CurrentSession != null;
    }
    
    public async Task Login(string email, string password)
    {
        await CloudDatabase?.SupabaseClient.Auth.SignIn(email, password)!;
    }
    
    public async Task Register(string email, string password, string username)
    {
        var signUpOptions = new SignUpOptions
        {
            Data = new Dictionary<string, object>
            {
                { "username", username }
            }
        };
        await CloudDatabase?.SupabaseClient.Auth.SignUp(email, password, signUpOptions)!;
    }
    
    public async Task Logout()
    {
        await CloudDatabase?.SupabaseClient.Auth.SignOut()!;
    }
    
    public string? GetCurrentUserId()
    {
        return CloudDatabase?.SupabaseClient.Auth.CurrentUser?.Id;
    }
    
    public string? GetCurrentUserEmail()
    {
        return CloudDatabase?.SupabaseClient.Auth.CurrentUser?.Email;
    }
}