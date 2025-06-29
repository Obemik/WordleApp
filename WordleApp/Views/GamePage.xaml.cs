using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordleApp.ViewModels;
using WordleApp.Services;
using WordleGameEngine.Enums;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

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
        _viewModel.GameGrid.CollectionChanged += GameGrid_CollectionChanged;

        // Enable keyboard input
        Focusable = true;
        Focus();
    
        Loaded += async (s, e) => 
        {
            Console.WriteLine($"[GamePage.Loaded] NewGameRequested: {_viewModel.NewGameRequested}");
        
            if (!_viewModel.NewGameRequested)
            {
                await _viewModel.InitializeAsync();
            
                await Task.Delay(100); 
                UpdateGameGrid();
                UpdateVirtualKeyboard();
                
                // Force keyboard to reset if it's a new game
                if (_viewModel.CurrentAttempt == 0)
                {
                    VirtualKeyboardControl.ResetKeyColors();
                }
            }
        
            _viewModel.NewGameRequested = false;
        };
    }
    
    private void GameGrid_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Console.WriteLine("[GamePage.GameGrid_CollectionChanged] Grid collection changed");
        UpdateEntireGrid();
    }
    
    private void UpdateEntireGrid()
    {
        Console.WriteLine("[GamePage.UpdateEntireGrid] Updating entire grid");
    
        for (int i = 0; i < _viewModel.GameGrid.Count; i++)
        {
            var guessModel = _viewModel.GameGrid[i];
            var word = string.Join("", guessModel.Letters.Select(l => string.IsNullOrEmpty(l.Letter) ? " " : l.Letter));
            var results = guessModel.Letters.Select(l => l.Status).ToArray();
        
            if (word.Trim().Length > 0)
            {
                Console.WriteLine($"[GamePage.UpdateEntireGrid] Row {i}: '{word}'");
            }
        
            GameGridControl.UpdateRow(i, word.PadRight(5), results);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GamePageViewModel.CurrentAttempt) || 
            e.PropertyName == nameof(GamePageViewModel.CurrentGuess) ||
            e.PropertyName == nameof(GamePageViewModel.GameGrid))
        {
            UpdateGameGrid();
            UpdateVirtualKeyboard(); 
        }

        if (e.PropertyName == nameof(GamePageViewModel.VirtualKeyboard))
        {
            UpdateVirtualKeyboard();
        }
    }

    private void UpdateGameGrid()
    {
        Console.WriteLine($"[GamePage.UpdateGameGrid] Updating grid with {_viewModel.GameGrid.Count} rows");
    
        for (int i = 0; i < _viewModel.GameGrid.Count; i++)
        {
            var guessModel = _viewModel.GameGrid[i];
        
            var word = "";
            var results = new GuessResult[5];
            bool hasContent = false;
        
            for (int j = 0; j < 5; j++)
            {
                if (j < guessModel.Letters.Count && !string.IsNullOrEmpty(guessModel.Letters[j].Letter))
                {
                    word += guessModel.Letters[j].Letter;
                    results[j] = guessModel.Letters[j].Status;
                    hasContent = true;
                }
                else
                {
                    word += " ";
                    results[j] = GuessResult.Absent;
                }
            }
        
            if (hasContent)
            {
                Console.WriteLine($"[GamePage.UpdateGameGrid] Row {i}: word='{word.Trim()}'");
            }
        
            // Оновлюємо рядок в GameGrid
            GameGridControl.UpdateRow(i, word, results);
        }
    
        if (_viewModel.CurrentAttempt < _viewModel.GameGrid.Count && !string.IsNullOrEmpty(_viewModel.CurrentGuess))
        {
            Console.WriteLine($"[GamePage.UpdateGameGrid] Current row {_viewModel.CurrentAttempt}: '{_viewModel.CurrentGuess}'");
            GameGridControl.UpdateCurrentRow(_viewModel.CurrentAttempt, _viewModel.CurrentGuess);
        }
    }

    private void UpdateVirtualKeyboard()
    {
        Console.WriteLine("[GamePage.UpdateVirtualKeyboard] Updating virtual keyboard UI");
    
        var keyStates = new Dictionary<string, GuessResult>();
    
        foreach (var key in _viewModel.VirtualKeyboard)
        {
            keyStates[key.Letter] = key.Status;
            if (key.Status != GuessResult.Absent)
            {
                Console.WriteLine($"[GamePage.UpdateVirtualKeyboard] Key '{key.Letter}' has status: {key.Status}");
            }
        }
    
        VirtualKeyboardControl.UpdateKeyColors(keyStates);
    }
    
    public void ForceRefreshKeyboard()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // First reset all keys completely
            VirtualKeyboardControl.ResetKeyColors();
            
            // Small delay to ensure reset is visible
            Task.Delay(50).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Then apply current state if needed
                    UpdateVirtualKeyboard();
                    
                    // Force visual update
                    VirtualKeyboardControl.InvalidateVisual();
                });
            });
        });
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
            
            // Force keyboard refresh after submitting guess
            await Task.Delay(50); // Small delay to ensure UI has updated
            ForceRefreshKeyboard();
        }
    }

    private void OnVirtualKeyboardBackspacePressed()
    {
        _viewModel.RemoveLetterCommand.Execute(null);
    }

    protected override async void OnKeyDown(KeyEventArgs e)
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
            if (_viewModel.SubmitGuessCommand.CanExecute(null))
            {
                await _viewModel.SubmitGuessAsync();
                
                // Force keyboard refresh after submitting guess
                await Task.Delay(50); // Small delay to ensure UI has updated
                ForceRefreshKeyboard();
            }
        }
    }

    private async void OnNewGameClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Ви впевнені, що хочете почати нову гру?", 
            "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            // Force complete keyboard reset
            VirtualKeyboardControl.ResetKeyColors();
            
            // Execute new game command
            if (_viewModel.NewGameCommand.CanExecute(null))
            {
                await ((AsyncRelayCommand)_viewModel.NewGameCommand).ExecuteAsync(null);
            }
            
            // Force refresh keyboard after game starts
            await Task.Delay(100);
            ForceRefreshKeyboard();
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