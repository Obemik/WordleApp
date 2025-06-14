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
    
    private Game? _currentGame;
    private GameDbModel? _currentGameDb;

    public event PropertyChangedEventHandler? PropertyChanged;

    public GameService(SupabaseRepository repository, GameEngineService gameEngine, AuthenticationService authService)
    {
        _repository = repository;
        _gameEngine = gameEngine;
        _authService = authService;
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
        if (_authService.CurrentUserId == null)
            throw new InvalidOperationException("User must be logged in to start a game");

        // Get random word from database
        var wordModel = await _repository.GetRandomWordAsync();
        if (wordModel == null)
            throw new InvalidOperationException("No words available in the database");

        // Create new game in database
        _currentGameDb = await _repository.CreateGameAsync(_authService.CurrentUserId, wordModel.Word);
        
        // Create game engine instance
        CurrentGame = _gameEngine.CreateGame(wordModel.Word);
        
        return CurrentGame;
    }

    public async Task<Game?> LoadCurrentGameAsync()
    {
        if (_authService.CurrentUserId == null) return null;

        _currentGameDb = await _repository.GetCurrentGameAsync(_authService.CurrentUserId);
        if (_currentGameDb == null) return null;

        // Deserialize guesses
        var guesses = JsonConvert.DeserializeObject<List<string>>(_currentGameDb.Guesses) ?? new List<string>();
        
        // Recreate game state
        CurrentGame = _gameEngine.CreateGame(_currentGameDb.TargetWord);
        
        // Apply previous guesses
        foreach (var guess in guesses)
        {
            _gameEngine.MakeGuess(CurrentGame, guess);
        }

        return CurrentGame;
    }

    public async Task<GuessResult[]> MakeGuessAsync(string guess)
    {
        if (CurrentGame == null || _currentGameDb == null)
            throw new InvalidOperationException("No active game");

        var result = _gameEngine.MakeGuess(CurrentGame, guess);
        
        // Update database
        var guesses = JsonConvert.DeserializeObject<List<string>>(_currentGameDb.Guesses) ?? new List<string>();
        guesses.Add(guess);
        
        _currentGameDb.Guesses = JsonConvert.SerializeObject(guesses);
        _currentGameDb.AttemptsCount = CurrentGame.Attempts;
        _currentGameDb.GameStatus = CurrentGame.Status.ToString();
        _currentGameDb.IsWon = CurrentGame.Status == GameStatus.Won;
        
        if (CurrentGame.Status != GameStatus.InProgress)
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

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}