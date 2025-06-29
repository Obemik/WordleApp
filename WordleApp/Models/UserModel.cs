using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WordleGameEngine.Enums;

namespace WordleApp.Models;

/// <summary>
/// Модель користувача для UI
/// </summary>
public class UserModel : INotifyPropertyChanged
{
    private string _id = string.Empty;
    private string _username = string.Empty;
    private string _email = string.Empty;
    private string _role = "Player";

    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
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