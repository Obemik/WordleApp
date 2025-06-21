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
            var app = (App)Application.Current;
            var gameService = app.Services.GetRequiredService<GameService>();
        
            gameService.ClearCache();
        
            _navigationService.NavigateTo<GamePage, GamePageViewModel>();
        
            await Task.Delay(100);
        
            if (app.MainWindow?.Content is GamePage gamePage && 
                gamePage.DataContext is GamePageViewModel gamePageViewModel)
            {
                await gamePageViewModel.InitializeNewGameAsync();
            }
        }
        catch (Exception ex)
        {
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