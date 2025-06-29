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
    private bool _isCreatingGame = false;

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

    public void OnUserChanged(object? sender, EventArgs e)
    {
        Console.WriteLine("[GameService.OnUserChanged] User changed, clearing all game data");
        ClearCache();
        CurrentGame = null;
        _currentGameDb = null;
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
        if (_isCreatingGame)
        {
            Console.WriteLine("[StartNewGameAsync] Already creating game, waiting...");
            int waitCount = 0;
            while (_isCreatingGame && waitCount < 50)
            {
                await Task.Delay(100);
                waitCount++;
            }
        
            if (_currentGame != null)
            {
                return _currentGame;
            }
        }

        try
        {
            _isCreatingGame = true;

            var currentUserId = _authService.CurrentUserId;
            if (string.IsNullOrEmpty(currentUserId))
                throw new InvalidOperationException("User must be logged in to start a game");

            Console.WriteLine($"[StartNewGameAsync] Starting new game for user: {currentUserId}");

            // Force clear all cached data
            ClearCache();

            // Get random word from database
            var wordModel = await _repository.GetRandomWordAsync();
            if (wordModel == null)
                throw new InvalidOperationException("No words available in the database");

            // Create new game in database
            _currentGameDb = await _repository.CreateGameAsync(currentUserId, wordModel.Word);
        
            // Create local game object
            CurrentGame = _gameEngine.CreateGame(wordModel.Word);
        
            Console.WriteLine($"[StartNewGameAsync] Created game ID: {_currentGameDb.Id} with word: {wordModel.Word}");
        
            return CurrentGame;
        }
        finally
        {
            _isCreatingGame = false;
        }
    }

    public async Task<Game?> LoadCurrentGameAsync()
    {
        var currentUserId = _authService.CurrentUserId;
        if (string.IsNullOrEmpty(currentUserId)) 
        {
            Console.WriteLine("[LoadCurrentGameAsync] No user ID, clearing cache");
            ClearCache();
            return null;
        }

        try
        {
            Console.WriteLine($"[LoadCurrentGameAsync] Loading game for user: {currentUserId}");
        
            _currentGameDb = await _repository.GetCurrentGameAsync(currentUserId);
        
            if (_currentGameDb == null) 
            {
                Console.WriteLine("[LoadCurrentGameAsync] No active game found");
                _currentGame = null;
                return null;
            }

            if (_currentGameDb.UserId != currentUserId)
            {
                Console.WriteLine($"[LoadCurrentGameAsync] User mismatch! Game userId: {_currentGameDb.UserId}, Current userId: {currentUserId}");
                ClearCache();
                return null;
            }

            // Deserialize guesses
            List<string> guesses;
            try
            {
                guesses = JsonConvert.DeserializeObject<List<string>>(_currentGameDb.Guesses) ?? new List<string>();
                Console.WriteLine($"[LoadCurrentGameAsync] Deserialized {guesses.Count} guesses: {string.Join(", ", guesses)}");
            }
            catch
            {
                guesses = new List<string>();
            }
        
            Console.WriteLine($"[LoadCurrentGameAsync] Loaded game ID: {_currentGameDb.Id}, Word: {_currentGameDb.TargetWord}, Guesses: {guesses.Count}");
        
            _currentGame = _gameEngine.CreateGame(_currentGameDb.TargetWord);
        
            // Apply previous guesses
            foreach (var guess in guesses)
            {
                var results = _gameEngine.MakeGuess(_currentGame, guess);
                Console.WriteLine($"[LoadCurrentGameAsync] Applied guess '{guess}' with results: {string.Join(",", results)}");
            }

            CurrentGame = _currentGame;
            Console.WriteLine($"[LoadCurrentGameAsync] Game loaded successfully with {_currentGame.Guesses.Count} guesses");
            
            return _currentGame;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LoadCurrentGameAsync] Error: {ex.Message}");
            Console.WriteLine($"[LoadCurrentGameAsync] Stack trace: {ex.StackTrace}");
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