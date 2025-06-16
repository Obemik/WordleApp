using SupabaseService.Models;
using Newtonsoft.Json;

namespace SupabaseService.Repository;

public partial class SupabaseRepository
{
    public async Task<GameDbModel> CreateGameAsync(string userId, string targetWord)
    {
        await EnsureInitializedAsync();
        var newGame = new GameDbModel
        {
            UserId = userId,
            TargetWord = targetWord,
            Guesses = "[]",
            IsWon = false,
            AttemptsCount = 0,
            GameStatus = "InProgress",
            StartedAt = DateTime.UtcNow
        };
        
        var result = await CloudDatabase!.SupabaseClient
            .From<GameDbModel>()
            .Insert(newGame);
        
        return result.Models.First();
    }
    
    public async Task<GameDbModel?> GetCurrentGameAsync(string userId)
    {
        await EnsureInitializedAsync();
        var result = await CloudDatabase!.SupabaseClient
            .From<GameDbModel>()
            .Where(g => g.UserId == userId && g.GameStatus == "InProgress")
            .Get();
        
        return result?.Models?.FirstOrDefault();
    }
    
    public async Task<GameDbModel> UpdateGameAsync(GameDbModel game)
    {
        await EnsureInitializedAsync();
        var result = await CloudDatabase!.SupabaseClient
            .From<GameDbModel>()
            .Where(g => g.Id == game.Id)
            .Set(g => g.Guesses, game.Guesses)
            .Set(g => g.AttemptsCount, game.AttemptsCount)
            .Set(g => g.GameStatus, game.GameStatus)
            .Set(g => g.IsWon, game.IsWon)
            .Set(g => g.CompletedAt, game.CompletedAt)
            .Update();
        
        return result.Models.First();
    }
    
    public async Task<List<GameDbModel>> GetUserGamesAsync(string userId, int limit = 20)
    {
        await EnsureInitializedAsync();
        var result = await CloudDatabase!.SupabaseClient
            .From<GameDbModel>()
            .Where(g => g.UserId == userId)
            .Order(g => g.StartedAt, Supabase.Postgrest.Constants.Ordering.Descending)
            .Limit(limit)
            .Get();
        
        return result?.Models ?? new List<GameDbModel>();
    }
}