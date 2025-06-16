using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WordleGameEngine.Enums;

namespace WordleApp.Models;

/// <summary>
/// Модель спроби відгадування для UI
/// </summary>
public class GuessModel : INotifyPropertyChanged
{
    public ObservableCollection<LetterModel> Letters { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Модель літери для UI
/// </summary>
public class LetterModel : INotifyPropertyChanged
{
    private string _letter = string.Empty;
    private GuessResult _status = GuessResult.Absent;

    public string Letter
    {
        get => _letter;
        set => SetProperty(ref _letter, value);
    }

    public GuessResult Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

/// <summary>
/// Модель клавіші для віртуальної клавіатури
/// </summary>
public class KeyModel : INotifyPropertyChanged
{
    private string _letter = string.Empty;
    private GuessResult _status = GuessResult.Absent;

    public string Letter
    {
        get => _letter;
        set => SetProperty(ref _letter, value);
    }

    public GuessResult Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}