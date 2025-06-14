using SupabaseService.Repository;

namespace WordleApp.Services;

public class WordValidationService
{
    private readonly SupabaseRepository _repository;

    public WordValidationService(SupabaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> IsValidWordAsync(string word)
    {
        if (string.IsNullOrWhiteSpace(word) || word.Length != 5)
            return false;

        return await _repository.WordExistsAsync(word);
    }

    public bool IsValidWordFormat(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return false;

        if (word.Length != 5)
            return false;

        // Check if all characters are letters
        return word.All(char.IsLetter);
    }

    public async Task<bool> AddWordAsync(string word, string? addedBy = null)
    {
        if (!IsValidWordFormat(word))
            return false;

        if (await _repository.WordExistsAsync(word))
            return false; // Word already exists

        try
        {
            await _repository.AddWordAsync(word, addedBy);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteWordAsync(int wordId)
    {
        try
        {
            return await _repository.DeleteWordAsync(wordId);
        }
        catch
        {
            return false;
        }
    }
}