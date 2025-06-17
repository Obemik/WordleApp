using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordleApp.ViewModels;
using WordleApp.Services;
using WordleGameEngine.Enums;

namespace WordleApp.Views;

public partial class GamePage : UserControl
{
    private readonly NavigationService _navigationService;
    private readonly GamePageViewModel _viewModel;

    public GamePage(NavigationService navigationService, GamePageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _navigationService = navigationService;
        _viewModel = viewModel;
        
        // Connect virtual keyboard events
        VirtualKeyboardControl.LetterPressed += OnVirtualKeyboardLetterPressed;
        VirtualKeyboardControl.EnterPressed += OnVirtualKeyboardEnterPressed;
        VirtualKeyboardControl.BackspacePressed += OnVirtualKeyboardBackspacePressed;
        
        // Subscribe to ViewModel property changes
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        
        // Enable keyboard input
        Focusable = true;
        Focus();
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GamePageViewModel.CurrentAttempt) || 
            e.PropertyName == nameof(GamePageViewModel.CurrentGuess))
        {
            UpdateGameGrid();
        }
        
        if (e.PropertyName == nameof(GamePageViewModel.VirtualKeyboard))
        {
            UpdateVirtualKeyboard();
        }
    }

    private void UpdateGameGrid()
    {
        // Update current row with typed letters
        if (_viewModel.CurrentAttempt < _viewModel.GameGrid.Count)
        {
            GameGridControl.UpdateCurrentRow(_viewModel.CurrentAttempt, _viewModel.CurrentGuess);
        }
        
        // Update completed rows
        for (int i = 0; i < _viewModel.CurrentAttempt && i < _viewModel.GameGrid.Count; i++)
        {
            var guessModel = _viewModel.GameGrid[i];
            var word = string.Join("", guessModel.Letters.Select(l => l.Letter));
            var results = guessModel.Letters.Select(l => l.Status).ToArray();
            
            if (!string.IsNullOrEmpty(word) && results.Any(r => r != GuessResult.Absent))
            {
                GameGridControl.UpdateRow(i, word, results);
            }
        }
    }

    private void UpdateVirtualKeyboard()
    {
        var keyStates = new Dictionary<string, GuessResult>();
        
        foreach (var key in _viewModel.VirtualKeyboard)
        {
            keyStates[key.Letter] = key.Status;
        }
        
        VirtualKeyboardControl.UpdateKeyColors(keyStates);
    }

    private void OnVirtualKeyboardLetterPressed(string letter)
    {
        _viewModel.AddLetterCommand.Execute(letter);
    }

    private async void OnVirtualKeyboardEnterPressed()
    {
        if (_viewModel.SubmitGuessCommand.CanExecute(null))
        {
            await _viewModel.SubmitGuessAsync();
        }
    }

    private void OnVirtualKeyboardBackspacePressed()
    {
        _viewModel.RemoveLetterCommand.Execute(null);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key >= Key.A && e.Key <= Key.Z)
        {
            // Add letter
            char letter = (char)('A' + (e.Key - Key.A));
            _viewModel.AddLetterCommand.Execute(letter.ToString());
        }
        else if (e.Key == Key.Back)
        {
            // Remove letter
            _viewModel.RemoveLetterCommand.Execute(null);
        }
        else if (e.Key == Key.Enter)
        {
            // Submit guess
            _viewModel.SubmitGuessCommand.Execute(null);
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