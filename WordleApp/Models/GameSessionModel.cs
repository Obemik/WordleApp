using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WordleGameEngine.Enums;

namespace WordleApp.Models;

/// <summary>
/// Модель ігрової сесії для UI
/// </summary>
public class GameSessionModel : INotifyPropertyChanged
{
    private int _id;
    private string _targetWord = string.Empty;
    private bool _isWon;
    private int _attemptsCount;
    private string _gameStatus = "InProgress";
    private DateTime _startedAt;
    private DateTime? _completedAt;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string TargetWord
    {
        get => _targetWord;
        set => SetProperty(ref _targetWord, value);
    }

    public bool IsWon
    {
        get => _isWon;
        set => SetProperty(ref _isWon, value);
    }

    public int AttemptsCount
    {
        get => _attemptsCount;
        set => SetProperty(ref _attemptsCount, value);
    }

    public string GameStatus
    {
        get => _gameStatus;
        set => SetProperty(ref _gameStatus, value);
    }

    public DateTime StartedAt
    {
        get => _startedAt;
        set => SetProperty(ref _startedAt, value);
    }

    public DateTime? CompletedAt
    {
        get => _completedAt;
        set => SetProperty(ref _completedAt, value);
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