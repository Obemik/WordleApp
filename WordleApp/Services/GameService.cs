using System.ComponentModel;
using System.Runtime.CompilerServices;
using SupabaseService.Repository;
using SupabaseService.Models;
using WordleGameEngine.Models;
using WordleGameEngine.Services;
using WordleGameEngine.Enums;
using Newtonsoft.Json;

namespace WordleApp.Services;

public class GameService : INotifyPropertyChanged
{
    private readonly SupabaseRepository _repository;
    private readonly GameEngineService _gameEngine;
    private readonly AuthenticationService _authService;
    private readonly WordValidationService _wordValidationService;
    
    private Game? _currentGame;
    private GameDbModel? _currentGameDb;

    public event PropertyChangedEventHandler? PropertyChanged;

    public GameService(SupabaseRepository repository, GameEngineService gameEngine, 
        AuthenticationService authService, WordValidationService wordValidationService)
    {
        _repository = repository;
        _gameEngine = gameEngine;
        _authService = authService;
        _wordValidationService = wordValidationService;
        
        _authService.UserChanged += OnUserChanged;
    }

    private void OnUserChanged(object? sender, EventArgs e)
    {
        ClearCache();
    }

    public Game? CurrentGame
    {
        get => _currentGame;
        private set
        {
            _currentGame = value;
            OnPropertyChanged();
        }
    }

    public async Task<Game> StartNewGameAsync()
    {
        var currentUserId = _authService.CurrentUserId;
        if (string.IsNullOrEmpty(currentUserId))
            throw new InvalidOperationException("User must be logged in to start a game");

        if (_currentGameDb != null && _currentGameDb.GameStatus == "InProgress")
        {
            _currentGameDb.GameStatus = "Abandoned";
            _currentGameDb.CompletedAt = DateTime.UtcNow;
            await _repository.UpdateGameAsync(_currentGameDb);
        }

        // Get random word from database
        var wordModel = await _repository.GetRandomWordAsync();
        if (wordModel == null)
            throw new InvalidOperationException("No words available in the database");

        // Create new game
        CurrentGame = _gameEngine.CreateGame(wordModel.Word);
    
        // Create new game in database для конкретного користувача
        _currentGameDb = await _repository.CreateGameAsync(currentUserId, wordModel.Word);
    
        Console.WriteLine($"Created new game for user {currentUserId} with word: {wordModel.Word}");
    
        return CurrentGame;
    }

    public async Task<Game?> LoadCurrentGameAsync()
    {
        var currentUserId = _authService.CurrentUserId;
        if (string.IsNullOrEmpty(currentUserId)) 
        {
            ClearCache();
            return null;
        }

        try
        {
            _currentGameDb = await _repository.GetCurrentGameAsync(currentUserId);
        
            if (_currentGameDb == null) 
            {
                _currentGame = null;
                return null;
            }

            if (_currentGameDb.UserId != currentUserId)
            {
                Console.WriteLine($"Game user mismatch: {_currentGameDb.UserId} != {currentUserId}");
                ClearCache();
                return null;
            }

            // Deserialize guesses
            List<string> guesses;
            try
            {
                guesses = JsonConvert.DeserializeObject<List<string>>(_currentGameDb.Guesses) ?? new List<string>();
            }
            catch
            {
                guesses = new List<string>();
            }
        
            Console.WriteLine($"Loading game for user {currentUserId} with word: {_currentGameDb.TargetWord}");
            _currentGame = _gameEngine.CreateGame(_currentGameDb.TargetWord);
        
            // Apply previous guesses
            foreach (var guess in guesses)
            {
                _gameEngine.MakeGuess(_currentGame, guess);
            }

            return _currentGame;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading game: {ex.Message}");
            ClearCache();
            return null;
        }
    }

    public async Task<GuessResult[]> MakeGuessAsync(string guess)
    {
        if (_currentGame == null || _currentGameDb == null)
            throw new InvalidOperationException("No active game");

        var currentUserId = _authService.CurrentUserId;
        if (string.IsNullOrEmpty(currentUserId) || _currentGameDb.UserId != currentUserId)
        {
            throw new InvalidOperationException("Game doesn't belong to current user");
        }

        // Make the guess
        var result = _gameEngine.MakeGuess(_currentGame, guess);
        
        // Update database
        var guesses = JsonConvert.DeserializeObject<List<string>>(_currentGameDb.Guesses) ?? new List<string>();
        guesses.Add(guess);
        
        _currentGameDb.Guesses = JsonConvert.SerializeObject(guesses);
        _currentGameDb.AttemptsCount = _currentGame.Attempts;
        _currentGameDb.GameStatus = _currentGame.Status.ToString();
        _currentGameDb.IsWon = _currentGame.Status == GameStatus.Won;
        
        if (_currentGame.Status != GameStatus.InProgress)
        {
            _currentGameDb.CompletedAt = DateTime.UtcNow;
        }

        await _repository.UpdateGameAsync(_currentGameDb);
        
        return result;
    }

    public bool IsValidWord(string word)
    {
        return _gameEngine.IsValidWord(word);
    }

    public void ClearCache()
    {
        _currentGame = null;
        _currentGameDb = null;
        OnPropertyChanged(nameof(CurrentGame));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}