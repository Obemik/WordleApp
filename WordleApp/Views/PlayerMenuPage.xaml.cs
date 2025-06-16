using System.Windows;
using System.Windows.Controls;
using WordleApp.ViewModels;
using WordleApp.Services;

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

    private void OnNewGameClick(object sender, RoutedEventArgs e)
    {
        _navigationService.NavigateTo<GamePage, GamePageViewModel>();
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