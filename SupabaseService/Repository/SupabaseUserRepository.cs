using SupabaseService.Models;

namespace SupabaseService.Repository;

public partial class SupabaseRepository
{
    public async Task<UserDbModel?> GetUserByIdAsync(string userId)
    {
        await EnsureInitializedAsync();
        var result = await CloudDatabase!.SupabaseClient
            .From<UserDbModel>()
            .Where(u => u.Id == userId)
            .Single();
        
        return result;
    }
    
    public async Task<UserDbModel?> GetUserByEmailAsync(string email)
    {
        await EnsureInitializedAsync();
        var result = await CloudDatabase!.SupabaseClient
            .From<UserDbModel>()
            .Where(u => u.Email == email)
            .Single();
        
        return result;
    }
    
    public async Task<UserDbModel> CreateUserAsync(string id, string username, string email, string role = "Player")
    {
        await EnsureInitializedAsync();
        var newUser = new UserDbModel
        {
            Id = id,
            Username = username,
            Email = email,
            Role = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var result = await CloudDatabase!.SupabaseClient
            .From<UserDbModel>()
            .Insert(newUser);
        
        return result.Models.First();
    }
    
    public async Task<UserDbModel> UpdateUserAsync(UserDbModel user)
    {
        await EnsureInitializedAsync();
        user.UpdatedAt = DateTime.UtcNow;
        
        var result = await CloudDatabase!.SupabaseClient
            .From<UserDbModel>()
            .Where(u => u.Id == user.Id)
            .Set(u => u.Username, user.Username)
            .Set(u => u.Email, user.Email)
            .Set(u => u.Role, user.Role)
            .Set(u => u.UpdatedAt, user.UpdatedAt)
            .Update();
        
        return result.Models.First();
    }
    
    public async Task<List<UserDbModel>> GetAllUsersAsync()
    {
        await EnsureInitializedAsync();
        var result = await CloudDatabase!.SupabaseClient
            .From<UserDbModel>()
            .Get();
        
        return result?.Models ?? new List<UserDbModel>();
    }
    
    public async Task<bool> DeleteUserAsync(string userId)
    {
        await EnsureInitializedAsync();
        try
        {
            await CloudDatabase!.SupabaseClient
                .From<UserDbModel>()
                .Where(u => u.Id == userId)
                .Delete();
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}