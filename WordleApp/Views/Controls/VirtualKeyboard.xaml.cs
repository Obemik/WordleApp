using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WordleGameEngine.Enums;
using WordleApp.Helpers;

namespace WordleApp.Views.Controls;

public partial class VirtualKeyboard : UserControl
{
    private readonly Dictionary<string, Button> _keyButtons;
    private bool _forceUpdate = false;
    
    public event System.Action<string>? LetterPressed;
    public event System.Action? EnterPressed;
    public event System.Action? BackspacePressed;

    public VirtualKeyboard()
    {
        InitializeComponent();
        _keyButtons = new Dictionary<string, Button>();
        InitializeKeyButtons();
    }

    private void InitializeKeyButtons()
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            var buttons = FindVisualChildren<Button>(this);
        
            foreach (var button in buttons)
            {
                var content = button.Content?.ToString();
                if (!string.IsNullOrEmpty(content) && content.Length == 1 && char.IsLetter(content[0]))
                {
                    _keyButtons[content] = button;
                    Console.WriteLine($"[VirtualKeyboard.InitializeKeyButtons] Registered key: {content}");
                }
            }
        }), System.Windows.Threading.DispatcherPriority.Loaded);
    }

    private void OnLetterClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Content is string letter)
        {
            LetterPressed?.Invoke(letter);
        }
    }

    private void OnEnterClick(object sender, RoutedEventArgs e)
    {
        EnterPressed?.Invoke();
    }

    private void OnBackspaceClick(object sender, RoutedEventArgs e)
    {
        BackspacePressed?.Invoke();
    }

    public void UpdateKeyColor(string letter, GuessResult result, bool forceUpdate = false)
    {
        if (_keyButtons.TryGetValue(letter.ToUpper(), out var button))
        {
            var color = ColorHelper.GetBrushFromGuessResult(result);
            var textColor = ColorHelper.GetTextBrushFromGuessResult(result);
        
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Get current status to check if we should update
                var currentBrush = button.Background as SolidColorBrush;
                var currentResult = GetGuessResultFromColor(currentBrush?.Color ?? Colors.Gray);
                
                // Force update if requested or follow priority rules
                bool shouldUpdate = forceUpdate || _forceUpdate;
                
                if (!shouldUpdate)
                {
                    if (currentResult == GuessResult.Absent)
                    {
                        shouldUpdate = true;
                    }
                    else if (currentResult == GuessResult.Present && result == GuessResult.Correct)
                    {
                        shouldUpdate = true;
                    }
                }
                
                if (shouldUpdate || result == currentResult)
                {
                    button.Background = color;
                    button.Foreground = textColor;
                    button.InvalidateVisual();
                    Console.WriteLine($"[VirtualKeyboard.UpdateKeyColor] Updated key '{letter}' from {currentResult} to {result}");
                }
                else
                {
                    Console.WriteLine($"[VirtualKeyboard.UpdateKeyColor] Skipped update for key '{letter}' - current: {currentResult}, new: {result}");
                }
            });
        }
        else
        {
            Console.WriteLine($"[VirtualKeyboard.UpdateKeyColor] Key '{letter}' not found in dictionary");
        }
    }

    public void UpdateKeyColors(Dictionary<string, GuessResult> letterResults)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            foreach (var kvp in letterResults)
            {
                UpdateKeyColor(kvp.Key, kvp.Value);
            }
        });
    }

    public void ResetKeyColors()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _forceUpdate = true; // Enable force update mode
            
            foreach (var kvp in _keyButtons)
            {
                var button = kvp.Value;
                button.Background = ColorHelper.GetBrushFromGuessResult(GuessResult.Absent);
                button.Foreground = Brushes.Black;
                button.InvalidateVisual();
            }
            
            _forceUpdate = false; // Disable force update mode
        });
    }

    public void SetEnabled(bool enabled)
    {
        foreach (var button in FindVisualChildren<Button>(this))
        {
            button.IsEnabled = enabled;
        }
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj == null) yield break;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);

            if (child is T t)
                yield return t;

            foreach (T childOfChild in FindVisualChildren<T>(child))
                yield return childOfChild;
        }
    }

    public Dictionary<string, GuessResult> GetCurrentKeyStates()
    {
        var states = new Dictionary<string, GuessResult>();
        
        foreach (var kvp in _keyButtons)
        {
            var button = kvp.Value;
            var background = button.Background as SolidColorBrush;
            
            if (background != null)
            {
                var result = GetGuessResultFromColor(background.Color);
                states[kvp.Key] = result;
            }
        }
        
        return states;
    }

    private GuessResult GetGuessResultFromColor(Color color)
    {
        if (color == ColorHelper.CorrectColor) return GuessResult.Correct;
        if (color == ColorHelper.PresentColor) return GuessResult.Present;
        if (color == ColorHelper.AbsentColor) return GuessResult.Absent;
        return GuessResult.Absent;
    }
}