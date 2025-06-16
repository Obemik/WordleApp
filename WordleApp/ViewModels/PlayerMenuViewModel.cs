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

    public PlayerMenuViewModel(AuthenticationService authService, NavigationService navigationService,
        GameService gameService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _gameService = gameService;

        NewGameCommand = new AsyncRelayCommand(StartNewGameAsync);
        ContinueGameCommand = new AsyncRelayCommand(ContinueGameAsync);
        LogoutCommand = new AsyncRelayCommand(LogoutAsync);

        LoadWelcomeMessage();
    }

    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }

    public ICommand NewGameCommand { get; }
    public ICommand ContinueGameCommand { get; }
    public ICommand LogoutCommand { get; }

    private async Task StartNewGameAsync()
    {
        try
        {
            await _gameService.StartNewGameAsync();
            // Navigation will be handled in the code-behind via NavigationService
        }
        catch (Exception ex)
        {
            // Handle error
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
        var userEmail = _authService.CurrentUserEmail;
        WelcomeMessage = $"Вітаємо, {userEmail}!";
    }
}    