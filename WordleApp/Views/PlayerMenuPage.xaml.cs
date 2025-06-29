using System.Windows;
using System.Windows.Controls;
using WordleApp.ViewModels;
using WordleApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace WordleApp.Views;

public partial class PlayerMenuPage : UserControl
{
    private readonly NavigationService _navigationService;

    public PlayerMenuPage(NavigationService navigationService, PlayerMenuViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _navigationService = navigationService;
    }

    private async void OnNewGameClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Console.WriteLine("[PlayerMenuPage.OnNewGameClick] Starting new game...");
            
            var app = (App)Application.Current;
            var gameService = app.Services.GetRequiredService<GameService>();
        
            // Clear game cache to ensure fresh start
            gameService.ClearCache();
            Console.WriteLine("[PlayerMenuPage.OnNewGameClick] Game cache cleared");
        
            // Navigate to game page
            _navigationService.NavigateTo<GamePage, GamePageViewModel>();
            Console.WriteLine("[PlayerMenuPage.OnNewGameClick] Navigated to GamePage");
        
            // Small delay to ensure navigation is complete
            await Task.Delay(150);
        
            if (app.MainWindow?.Content is GamePage gamePage && 
                gamePage.DataContext is GamePageViewModel gamePageViewModel)
            {
                Console.WriteLine("[PlayerMenuPage.OnNewGameClick] Initializing new game...");
                
                // Initialize new game
                await gamePageViewModel.InitializeNewGameAsync();
                
                // Force refresh keyboard
                gamePage.ForceRefreshKeyboard();
                
                Console.WriteLine("[PlayerMenuPage.OnNewGameClick] New game initialized successfully");
            }
            else
            {
                Console.WriteLine("[PlayerMenuPage.OnNewGameClick] Failed to get GamePage or GamePageViewModel");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PlayerMenuPage.OnNewGameClick] Error: {ex.Message}");
            MessageBox.Show($"Помилка при створенні нової гри: {ex.Message}", "Помилка", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void OnContinueGameClick(object sender, RoutedEventArgs e)
    {
        _navigationService.NavigateTo<GamePage, GamePageViewModel>();
    }

    private void OnLogoutClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Ви впевнені, що хочете вийти?", "Підтвердження", 
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            if (DataContext is PlayerMenuViewModel viewModel)
            {
                viewModel.LogoutCommand.Execute(null);
            }
            _navigationService.NavigateTo<AuthenticationPage, AuthenticationViewModel>();
        }
    }
}