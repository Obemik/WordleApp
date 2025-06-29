using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WordleGameEngine.Enums;

namespace WordleApp.Models;

/// <summary>
/// Модель слова для UI
/// </summary>
public class WordModel : INotifyPropertyChanged
{
    private int _id;
    private string _word = string.Empty;
    private bool _isActive = true;
    private DateTime _createdAt;
    private string? _addedBy;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Word
    {
        get => _word;
        set => SetProperty(ref _word, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set => SetProperty(ref _createdAt, value);
    }

    public string? AddedBy
    {
        get => _addedBy;
        set => SetProperty(ref _addedBy, value);
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