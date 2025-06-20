using SupabaseService.Models;
using Newtonsoft.Json;

namespace SupabaseService.Repository;

public partial class SupabaseRepository
{
    public async Task<GameDbModel> CreateGameAsync(string userId, string targetWord)
    {
        await EnsureInitializedAsync();
        
        var activeGames = await CloudDatabase!.SupabaseClient
            .From<GameDbModel>()
            .Where(g => g.UserId == userId)
            .Where(g => g.GameStatus == "InProgress")
            .Get();
        
        foreach (var activeGame in activeGames.Models)
        {
            activeGame.GameStatus = "Abandoned";
            activeGame.CompletedAt = DateTime.UtcNow;
            
            await CloudDatabase!.SupabaseClient
                .From<GameDbModel>()
                .Where(g => g.Id == activeGame.Id)
                .Set(g => g.GameStatus, "Abandoned")
                .Set(g => g.CompletedAt, DateTime.UtcNow)
                .Update();
        }
        
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
        
        try
        {
            Console.WriteLine($"[GetCurrentGameAsync] Searching for active game for user: {userId}");
            
            var result = await CloudDatabase!.SupabaseClient
                .From<GameDbModel>()
                .Where(g => g.UserId == userId)
                .Where(g => g.GameStatus == "InProgress")
                .Order(g => g.StartedAt, Supabase.Postgrest.Constants.Ordering.Descending)
                .Limit(1)
                .Get();
            
            var game = result?.Models?.FirstOrDefault();
            
            if (game != null)
            {
                Console.WriteLine($"[GetCurrentGameAsync] Found game ID: {game.Id}, Word: {game.TargetWord}, Status: {game.GameStatus}");
            }
            else
            {
                Console.WriteLine($"[GetCurrentGameAsync] No active game found for user: {userId}");
            }
            
            return game;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetCurrentGameAsync] Error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<GameDbModel> UpdateGameAsync(GameDbModel game)
    {
        await EnsureInitializedAsync();
        
        Console.WriteLine($"[UpdateGameAsync] Updating game ID: {game.Id}, Status: {game.GameStatus}");
        
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
}