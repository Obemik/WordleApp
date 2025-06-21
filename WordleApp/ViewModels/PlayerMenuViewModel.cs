using System.Windows.Input;
using WordleApp.ViewModels;
using WordleApp.Services;
using WordleApp.Helpers;
using CommunityToolkit.Mvvm.Input;

namespace WordleApp.ViewModels;

public class PlayerMenuViewModel : BaseViewModel
{
    private readonly AuthenticationService _authService;
    private readonly NavigationService _navigationService;
    private readonly GameService _gameService;

    private string _welcomeMessage = string.Empty;
    private bool _hasActiveGame = false;

    public PlayerMenuViewModel(AuthenticationService authService, NavigationService navigationService,
        GameService gameService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _gameService = gameService;

        NewGameCommand = new AsyncRelayCommand(StartNewGameAsync);
        ContinueGameCommand = new AsyncRelayCommand(ContinueGameAsync, CanContinueGame);
        LogoutCommand = new AsyncRelayCommand(LogoutAsync);

        // Subscribe to changes in CurrentUser
        _authService.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(AuthenticationService.CurrentUser))
            {
                LoadWelcomeMessage();
            }
        };

        LoadWelcomeMessage();
        CheckActiveGame();
    }

    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }

    public bool HasActiveGame
    {
        get => _hasActiveGame;
        set
        {
            if (SetProperty(ref _hasActiveGame, value))
            {
                ((AsyncRelayCommand)ContinueGameCommand).NotifyCanExecuteChanged();
            }
        }
    }

    public ICommand NewGameCommand { get; }
    public ICommand ContinueGameCommand { get; }
    public ICommand LogoutCommand { get; }
    
    private async Task StartNewGameAsync()
    {
        try
        {
            HasActiveGame = true; 
            // Navigation will be handled in the code-behind via NavigationService
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PlayerMenuViewModel.StartNewGameAsync] Error: {ex.Message}");
        }
    }

    private async Task ContinueGameAsync()
    {
        try
        {
            var game = await _gameService.LoadCurrentGameAsync();
            if (game != null)
            {
                // Navigation will be handled in the code-behind via NavigationService
            }
        }
        catch (Exception ex)
        {
            // Handle error
        }
    }

    private bool CanContinueGame()
    {
        return HasActiveGame;
    }

    private async Task LogoutAsync()
    {
        try
        {
            await _authService.LogoutAsync();
            // Navigation will be handled in the code-behind via NavigationService
        }
        catch (Exception ex)
        {
            // Handle error
        }
    }

    private void LoadWelcomeMessage()
    {
        var username = _authService.CurrentUsername;
        if (!string.IsNullOrEmpty(username))
        {
            WelcomeMessage = $"Вітаємо, {username}!";
        }
        else
        {
            var userEmail = _authService.CurrentUserEmail;
            WelcomeMessage = $"Вітаємо, {userEmail}!";
        }
    }

    private async void CheckActiveGame()
    {
        try
        {
            var currentUserId = _authService.CurrentUserId;
            Console.WriteLine($"[PlayerMenuViewModel.CheckActiveGame] Checking for user: {currentUserId}");
        
            _gameService.ClearCache();
        
            if (!string.IsNullOrEmpty(currentUserId))
            {
                var game = await _gameService.LoadCurrentGameAsync();
                HasActiveGame = game != null && !game.IsGameOver;
            
                Console.WriteLine($"[PlayerMenuViewModel.CheckActiveGame] HasActiveGame: {HasActiveGame}");
            }
            else
            {
                HasActiveGame = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PlayerMenuViewModel.CheckActiveGame] Error: {ex.Message}");
            HasActiveGame = false;
        }
    }
}