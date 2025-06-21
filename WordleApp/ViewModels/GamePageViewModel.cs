using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;
using WordleApp.ViewModels;
using WordleApp.Services;
using WordleApp.Models;
using WordleGameEngine.Models;
using WordleGameEngine.Enums;
using CommunityToolkit.Mvvm.Input;

namespace WordleApp.ViewModels;

public class GamePageViewModel : BaseViewModel
{
    private bool _isInitializing = false;
    private readonly GameService _gameService;
    private readonly NavigationService _navigationService;
    private readonly WordValidationService _wordValidationService;
    private readonly AuthenticationService _authService;
    
    private string _currentGuess = string.Empty;
    private string _targetWord = string.Empty;
    private string _gameStatus = string.Empty;
    private bool _isGameOver = false;
    private int _currentAttempt = 0;
    private int _maxAttempts = 6;
    private bool _isLoading = false;
    private bool _newGameRequested = false;
    
    public bool NewGameRequested
    {
        get => _newGameRequested;
        set => SetProperty(ref _newGameRequested, value);
    }
    
    public GamePageViewModel(GameService gameService, NavigationService navigationService, 
        WordValidationService wordValidationService, AuthenticationService authService)
    {
        _authService = authService;
        _gameService = gameService;
        _navigationService = navigationService;
        _wordValidationService = wordValidationService;

        Console.WriteLine($"[GamePageViewModel] Created for user: {_authService.CurrentUserId}");

        GameGrid = new ObservableCollection<GuessModel>();
        VirtualKeyboard = new ObservableCollection<KeyModel>();

        SubmitGuessCommand = new AsyncRelayCommand(SubmitGuessAsync, CanSubmitGuess);
        AddLetterCommand = new RelayCommand<string>(AddLetter);
        RemoveLetterCommand = new RelayCommand(RemoveLetter, CanRemoveLetter);
        NewGameCommand = new AsyncRelayCommand(StartNewGameAsync);

        InitializeGameGrid();
        InitializeVirtualKeyboard();
    }

    public async Task InitializeAsync()
    {
        await LoadOrStartNewGame();
    }
    
    public async Task InitializeNewGameAsync()
    {
        NewGameRequested = true;
        
        // Reset game state first
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            ResetGame();
            CurrentGuess = string.Empty;
            CurrentAttempt = 0;
            IsGameOver = false;
            GameStatus = string.Empty;
        });
        
        // Start new game
        await StartNewGameAsync();
        
        // Force UI update after new game started
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            UpdateGameGrid(string.Empty, Array.Empty<GuessResult>());
            OnPropertyChanged(nameof(VirtualKeyboard));
        });
    }

    public ObservableCollection<GuessModel> GameGrid { get; }
    public ObservableCollection<KeyModel> VirtualKeyboard { get; }

    public string CurrentGuess
    {
        get => _currentGuess;
        set
        {
            if (SetProperty(ref _currentGuess, value))
            {
                ((AsyncRelayCommand)SubmitGuessCommand).NotifyCanExecuteChanged();
                ((RelayCommand)RemoveLetterCommand).NotifyCanExecuteChanged();
                OnPropertyChanged();
            }
        }
    }

    public string GameStatus
    {
        get => _gameStatus;
        set => SetProperty(ref _gameStatus, value);
    }

    public bool IsGameOver
    {
        get => _isGameOver;
        set
        {
            if (SetProperty(ref _isGameOver, value))
            {
                OnPropertyChanged(nameof(AttemptText)); 
            }
        }
    }

    public int CurrentAttempt
    {
        get => _currentAttempt;
        set
        {
            if (SetProperty(ref _currentAttempt, value))
            {
                OnPropertyChanged(nameof(AttemptText));
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string AttemptText
    {
        get
        {
            if (IsGameOver)
            {
                return string.Empty; 
            }
            return $"Спроба {CurrentAttempt + 1} з {_maxAttempts}";
        }
    }

    public ICommand SubmitGuessCommand { get; }
    public ICommand AddLetterCommand { get; }
    public ICommand RemoveLetterCommand { get; }
    public ICommand NewGameCommand { get; }

    public async Task SubmitGuessAsync()
    {
        try
        {
            IsLoading = true;
            
            var isValidWord = await _wordValidationService.IsValidWordAsync(CurrentGuess);
            if (!isValidWord)
            {
                GameStatus = "Такого слова немає в словнику!";
                IsLoading = false;
                return;
            }

            var results = await _gameService.MakeGuessAsync(CurrentGuess);
            
            // Update game grid
            UpdateGameGrid(CurrentGuess, results);
            
            // Update virtual keyboard colors
            UpdateVirtualKeyboard(CurrentGuess, results);
            
            // Force UI refresh
            OnPropertyChanged(nameof(GameGrid));
            OnPropertyChanged(nameof(VirtualKeyboard));

            CurrentAttempt++;
            CurrentGuess = string.Empty;

            // Check game status
            var game = _gameService.CurrentGame;
            if (game != null)
            {
                IsGameOver = game.IsGameOver;
                
                if (game.IsWon)
                {
                    GameStatus = $"Вітаємо! Ви відгадали слово за {game.Attempts} спроб!";
                }
                else if (game.IsLost)
                {
                    GameStatus = $"Гра завершена. Слово було: {game.TargetWord}";
                }
                else
                {
                    GameStatus = "Продовжуйте відгадувати!";
                }
            }
        }
        catch (Exception ex)
        {
            GameStatus = $"Помилка: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddLetter(string? letter)
    {
        if (letter != null && CurrentGuess.Length < 5 && !IsGameOver && !IsLoading)
        {
            CurrentGuess += letter.ToUpper();
        }
    }

    private void RemoveLetter()
    {
        if (CurrentGuess.Length > 0)
        {
            CurrentGuess = CurrentGuess[..^1];
        }
    }

    private async Task StartNewGameAsync()
    {
        if (_isInitializing || IsLoading) return;
    
        try
        {
            IsLoading = true;
        
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ResetGame();
                CurrentGuess = string.Empty;
                CurrentAttempt = 0;
                IsGameOver = false;
                GameStatus = string.Empty;
                
                OnPropertyChanged(nameof(GameGrid));
                OnPropertyChanged(nameof(VirtualKeyboard));
            });
        
            await _gameService.StartNewGameAsync();
        
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                GameStatus = "Нова гра почалась! Почніть відгадувати!";
                UpdateGameGrid(string.Empty, Array.Empty<GuessResult>());
                
                var keyStates = new Dictionary<string, GuessResult>();
                foreach (var key in VirtualKeyboard)
                {
                    keyStates[key.Letter] = key.Status;
                }
                OnPropertyChanged(nameof(VirtualKeyboard));
            });
        }
        catch (Exception ex)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                GameStatus = $"Помилка створення нової гри: {ex.Message}";
            });
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanSubmitGuess()
    {
        return CurrentGuess.Length == 5 && !IsGameOver && !IsLoading;
    }

    private bool CanRemoveLetter()
    {
        return CurrentGuess.Length > 0 && !IsGameOver && !IsLoading;
    }

    private void InitializeGameGrid()
    {
        GameGrid.Clear();
        for (int i = 0; i < _maxAttempts; i++)
        {
            var guess = new GuessModel();
            for (int j = 0; j < 5; j++)
            {
                guess.Letters.Add(new LetterModel());
            }
            GameGrid.Add(guess);
        }
    }

    private void InitializeVirtualKeyboard()
    {
        var keys = "QWERTYUIOPASDFGHJKLZXCVBNM";
        VirtualKeyboard.Clear();
        foreach (char key in keys)
        {
            VirtualKeyboard.Add(new KeyModel { Letter = key.ToString(), Status = GuessResult.Absent });
        }
    }

    private void UpdateGameGrid(string guess, GuessResult[] results)
    {
        if (CurrentAttempt < GameGrid.Count)
        {
            var guessModel = GameGrid[CurrentAttempt];
            for (int i = 0; i < guess.Length && i < guessModel.Letters.Count; i++)
            {
                guessModel.Letters[i].Letter = guess[i].ToString();
                guessModel.Letters[i].Status = results[i];
            }
        }
    }

    private void UpdateVirtualKeyboard(string guess, GuessResult[] results)
    {
        Console.WriteLine($"[GamePageViewModel.UpdateVirtualKeyboard] Updating keyboard for word: {guess}");
    
        for (int i = 0; i < guess.Length; i++)
        {
            var letter = guess[i].ToString();
            var key = VirtualKeyboard.FirstOrDefault(k => k.Letter == letter);
        
            if (key != null)
            {
                var oldStatus = key.Status;
                
                // Priority: Correct > Present > Absent
                // Once a key is Correct, it should never change
                // Once a key is Present, it can only change to Correct
                if (key.Status == GuessResult.Correct)
                {
                    // Already correct, don't change
                    continue;
                }
                else if (key.Status == GuessResult.Present)
                {
                    // Can only upgrade to Correct
                    if (results[i] == GuessResult.Correct)
                    {
                        key.Status = GuessResult.Correct;
                    }
                }
                else // key.Status == GuessResult.Absent
                {
                    // Can upgrade to Present or Correct
                    key.Status = results[i];
                }
            
                if (oldStatus != key.Status)
                {
                    Console.WriteLine($"[GamePageViewModel.UpdateVirtualKeyboard] Key '{letter}' status changed from {oldStatus} to {key.Status}");
                }
            }
        }
        
        // Force UI update
        OnPropertyChanged(nameof(VirtualKeyboard));
    }
    public void RefreshVirtualKeyboard()
    {
        OnPropertyChanged(nameof(VirtualKeyboard));
    }
    private async Task LoadOrStartNewGame()
    {
        if (_isInitializing) return;

        try
        {
            _isInitializing = true;
            IsLoading = true;
            
            var currentUserId = _authService.CurrentUserId;
            Console.WriteLine($"[GamePageViewModel.LoadOrStartNewGame] Loading for user: {currentUserId}");
            
            await Application.Current.Dispatcher.InvokeAsync(() => ResetGame());
            
            var game = await _gameService.LoadCurrentGameAsync();
            
            if (game != null && !game.IsGameOver)
            {
                Console.WriteLine($"[GamePageViewModel.LoadOrStartNewGame] Found active game with {game.Guesses.Count} guesses");
                
                for (int i = 0; i < game.Guesses.Count; i++)
                {
                    Console.WriteLine($"[GamePageViewModel.LoadOrStartNewGame] Guess {i}: '{game.Guesses[i].Word}' - {string.Join(",", game.Guesses[i].Results)}");
                }
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // Restore game state
                    CurrentAttempt = game.Attempts;
                    IsGameOver = game.IsGameOver;
                    
                    Console.WriteLine($"[GamePageViewModel.LoadOrStartNewGame] Restoring {game.Guesses.Count} guesses to grid");
                    
                    // First reset the keyboard to ensure clean state
                    foreach (var key in VirtualKeyboard)
                    {
                        key.Status = GuessResult.Absent;
                    }
                    
                    for (int i = 0; i < game.Guesses.Count && i < GameGrid.Count; i++)
                    {
                        var guess = game.Guesses[i];
                        
                        Console.WriteLine($"[GamePageViewModel.LoadOrStartNewGame] Restoring row {i} with word '{guess.Word}'");
                        
                        for (int j = 0; j < guess.Word.Length && j < GameGrid[i].Letters.Count; j++)
                        {
                            GameGrid[i].Letters[j].Letter = guess.Word[j].ToString();
                            GameGrid[i].Letters[j].Status = guess.Results[j];
                        }
                        
                        UpdateVirtualKeyboard(guess.Word, guess.Results);
                    }

                    GameStatus = "Продовжуйте відгадувати!";
                    
                    OnPropertyChanged(nameof(GameGrid));
                    OnPropertyChanged(nameof(CurrentAttempt));
                    OnPropertyChanged(nameof(AttemptText));
                    OnPropertyChanged(nameof(VirtualKeyboard));
                    
                    Console.WriteLine("[GamePageViewModel.LoadOrStartNewGame] UI update completed");
                    VerifyGridState();
            
                    Console.WriteLine("[GamePageViewModel.LoadOrStartNewGame] UI update completed");
                });
            }
            else
            {
                Console.WriteLine("[GamePageViewModel.LoadOrStartNewGame] No active game, starting new one");
                await StartNewGameAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GamePageViewModel.LoadOrStartNewGame] Error: {ex.Message}");
            Console.WriteLine($"[GamePageViewModel.LoadOrStartNewGame] Stack trace: {ex.StackTrace}");
            
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                GameStatus = $"Помилка завантаження гри";
            });
        }
        finally
        {
            IsLoading = false;
            _isInitializing = false;
        }
    }

    private void VerifyGridState()
    {
        Console.WriteLine("[GamePageViewModel.VerifyGridState] Checking grid state:");
    
        for (int i = 0; i < GameGrid.Count; i++)
        {
            var row = GameGrid[i];
            var letters = string.Join("", row.Letters.Select(l => string.IsNullOrEmpty(l.Letter) ? "_" : l.Letter));
        
            if (letters.Replace("_", "").Length > 0)
            {
                var statuses = string.Join(",", row.Letters.Select(l => l.Status.ToString().Substring(0, 1)));
                Console.WriteLine($"  Row {i}: '{letters}' [{statuses}]");
            }
        }
    }

    private void ResetGame()
    {
        CurrentGuess = string.Empty;
        CurrentAttempt = 0;
        IsGameOver = false;
        GameStatus = string.Empty;
    
        foreach (var guessRow in GameGrid)
        {
            foreach (var letter in guessRow.Letters)
            {
                letter.Letter = "";
                letter.Status = GuessResult.Absent;
            }
        }
    
        foreach (var key in VirtualKeyboard)
        {
            key.Status = GuessResult.Absent;
        }
    
        OnPropertyChanged(nameof(GameGrid));
        OnPropertyChanged(nameof(VirtualKeyboard));
        
        // Force virtual keyboard to refresh its visual state
        RefreshVirtualKeyboard();
    }
}

