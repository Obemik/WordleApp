using System.Windows.Input;
using System.Collections.ObjectModel;
using WordleApp.ViewModels;
using WordleApp.Services;
using WordleApp.Models;
using WordleGameEngine.Models;
using WordleGameEngine.Enums;
using CommunityToolkit.Mvvm.Input;

namespace WordleApp.ViewModels;

public class GamePageViewModel : BaseViewModel
{
    private readonly GameService _gameService;
    private readonly NavigationService _navigationService;
    private readonly WordValidationService _wordValidationService;
    
    private string _currentGuess = string.Empty;
    private string _targetWord = string.Empty;
    private string _gameStatus = string.Empty;
    private bool _isGameOver = false;
    private int _currentAttempt = 0;
    private int _maxAttempts = 6;
    private bool _isLoading = false;

    public GamePageViewModel(GameService gameService, NavigationService navigationService, WordValidationService wordValidationService)
    {
        _gameService = gameService;
        _navigationService = navigationService;
        _wordValidationService = wordValidationService;

        GameGrid = new ObservableCollection<GuessModel>();
        VirtualKeyboard = new ObservableCollection<KeyModel>();

        SubmitGuessCommand = new AsyncRelayCommand(SubmitGuessAsync, CanSubmitGuess);
        AddLetterCommand = new RelayCommand<string>(AddLetter);
        RemoveLetterCommand = new RelayCommand(RemoveLetter, CanRemoveLetter);
        NewGameCommand = new AsyncRelayCommand(StartNewGameAsync);

        InitializeGameGrid();
        InitializeVirtualKeyboard();
        _ = LoadOrStartNewGame();
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
        set => SetProperty(ref _isGameOver, value);
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

    public string AttemptText => $"Спроба {CurrentAttempt + 1} з {_maxAttempts}";

    public ICommand SubmitGuessCommand { get; }
    public ICommand AddLetterCommand { get; }
    public ICommand RemoveLetterCommand { get; }
    public ICommand NewGameCommand { get; }

    private async Task SubmitGuessAsync()
    {
        try
        {
            IsLoading = true;
            
            // Перевіряємо чи слово валідне
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
        try
        {
            IsLoading = true;
            await _gameService.StartNewGameAsync();
            ResetGame();
            GameStatus = "Нова гра почалась! Почніть відгадувати!";
        }
        catch (Exception ex)
        {
            GameStatus = $"Помилка створення нової гри: {ex.Message}";
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
        for (int i = 0; i < guess.Length; i++)
        {
            var key = VirtualKeyboard.FirstOrDefault(k => k.Letter == guess[i].ToString());
            if (key != null)
            {
                // Only update if the new status is better than the current one
                if (results[i] > key.Status)
                {
                    key.Status = results[i];
                }
            }
        }
    }

    private async Task LoadOrStartNewGame()
    {
        try
        {
            IsLoading = true;
            var game = await _gameService.LoadCurrentGameAsync();
            
            if (game != null)
            {
                // Restore game state
                _targetWord = game.TargetWord;
                CurrentAttempt = game.Attempts;
                IsGameOver = game.IsGameOver;
                
                // Restore previous guesses
                for (int i = 0; i < game.Guesses.Count; i++)
                {
                    var guess = game.Guesses[i];
                    UpdateGameGrid(guess.Word, guess.Results);
                    UpdateVirtualKeyboard(guess.Word, guess.Results);
                }

                if (game.IsWon)
                {
                    GameStatus = $"Ви відгадали слово за {game.Attempts} спроб!";
                }
                else if (game.IsLost)
                {
                    GameStatus = $"Слово було: {game.TargetWord}";
                }
                else
                {
                    GameStatus = "Продовжуйте відгадувати!";
                }
            }
            else
            {
                // Start new game
                await StartNewGameAsync();
            }
        }
        catch (Exception ex)
        {
            GameStatus = $"Помилка завантаження гри: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ResetGame()
    {
        CurrentGuess = string.Empty;
        CurrentAttempt = 0;
        IsGameOver = false;
        
        InitializeGameGrid();
        InitializeVirtualKeyboard();
    }
}