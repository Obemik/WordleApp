using SupabaseService.Repository;

namespace WordleApp.Services;

public class WordValidationService
{
    private readonly SupabaseRepository _repository;
    private List<string>? _cachedWords = null;
    private DateTime _cacheExpiry = DateTime.MinValue;

    public WordValidationService(SupabaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> IsValidWordAsync(string word)
    {
        if (string.IsNullOrWhiteSpace(word) || word.Length != 5)
            return false;

        if (!word.All(char.IsLetter))
            return false;

        // Use cached words if available and not expired
        if (_cachedWords != null && DateTime.Now < _cacheExpiry)
        {
            return _cachedWords.Contains(word.ToUpper());
        }

        // Refresh cache
        try
        {
            var words = await _repository.GetAllWordsAsync();
            _cachedWords = words.Select(w => w.Word).ToList();
            _cacheExpiry = DateTime.Now.AddMinutes(5); // Cache for 5 minutes
            
            return _cachedWords.Contains(word.ToUpper());
        }
        catch
        {
            // Fallback to checking each time
            return await _repository.CheckWordInDictionary(word);
        }
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

        // Clear cache when adding new word
        _cachedWords = null;

        try
        {
            // Check if word exists using cached method
            var exists = await IsValidWordAsync(word);
            if (exists)
                return false;

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
        // Clear cache when deleting word
        _cachedWords = null;

        try
        {
            return await _repository.DeleteWordAsync(wordId);
        }
        catch
        {
            return false;
        }
    }

    public void ClearCache()
    {
        _cachedWords = null;
        _cacheExpiry = DateTime.MinValue;
    }
}