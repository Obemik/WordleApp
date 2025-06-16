using SupabaseService.Models;

namespace SupabaseService.Repository;

public partial class SupabaseRepository
{
    public async Task<List<WordDbModel>> GetAllWordsAsync()
    {
        await EnsureInitializedAsync();
        var result = await CloudDatabase!.SupabaseClient
            .From<WordDbModel>()
            .Where(w => w.IsActive == true)
            .Get();
        return result?.Models ?? new List<WordDbModel>();
    }
    
    public async Task<WordDbModel?> GetRandomWordAsync()
    {
        await EnsureInitializedAsync();
        var words = await GetAllWordsAsync();
        if (words.Count == 0) return null;
        
        var random = new Random();
        return words[random.Next(words.Count)];
    }
    
    public async Task<WordDbModel> AddWordAsync(string word, string? addedBy = null)
    {
        await EnsureInitializedAsync();
        var newWord = new WordDbModel
        {
            Word = word.ToUpper(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            AddedBy = addedBy
        };
        
        var result = await CloudDatabase!.SupabaseClient
            .From<WordDbModel>()
            .Insert(newWord);
        
        return result.Models.First();
    }
    
    public async Task<bool> DeleteWordAsync(int wordId)
    {
        await EnsureInitializedAsync();
        try
        {
            await CloudDatabase!.SupabaseClient
                .From<WordDbModel>()
                .Where(w => w.Id == wordId)
                .Delete();
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> WordExistsAsync(string word)
    {
        await EnsureInitializedAsync();
        var result = await CloudDatabase!.SupabaseClient
            .From<WordDbModel>()
            .Where(w => w.Word == word.ToUpper() && w.IsActive == true)
            .Get();
        
        return result?.Models?.Count > 0;
    }
}