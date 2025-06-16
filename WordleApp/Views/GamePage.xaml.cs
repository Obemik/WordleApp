using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordleApp.ViewModels;
using WordleApp.Services;

namespace WordleApp.Views;

public partial class GamePage : UserControl
{
    private readonly NavigationService _navigationService;

    public GamePage(NavigationService navigationService, GamePageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _navigationService = navigationService;
        
        // Enable keyboard input
        Focusable = true;
        Focus();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (DataContext is GamePageViewModel viewModel)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                // Add letter
                char letter = (char)('A' + (e.Key - Key.A));
                viewModel.AddLetterCommand.Execute(letter.ToString());
            }
            else if (e.Key == Key.Back)
            {
                // Remove letter
                viewModel.RemoveLetterCommand.Execute(null);
            }
            else if (e.Key == Key.Enter)
            {
                // Submit guess
                viewModel.SubmitGuessCommand.Execute(null);
            }
        }
    }

    private void OnBackToMenuClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Ви впевнені, що хочете вийти з гри? Прогрес буде збережений.", 
            "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            _navigationService.NavigateTo<PlayerMenuPage, PlayerMenuViewModel>();
        }
    }
}