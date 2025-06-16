using System.ComponentModel;
using System.Runtime.CompilerServices;
using SupabaseService.Repository;

namespace WordleApp.Services;

public class AuthenticationService : INotifyPropertyChanged
{
    private readonly SupabaseRepository _repository;
    private bool _isLoggedIn;
    private bool _isRegisteredSuccessfully = false;
    private bool _isInitialized = false;

    public event PropertyChangedEventHandler? PropertyChanged;
    
    public AuthenticationService(SupabaseRepository repository)
    {
        _repository = repository;
        InitializeAsync();
    }
    
    private async void InitializeAsync()
    {
        await _repository.InitializeAsync();
        _isInitialized = true;
        IsLoggedIn = _repository.IsLoggedIn;
    }
    
    public bool IsRegisteredSuccessfully
    {
        get => _isRegisteredSuccessfully;
        set
        {
            if (_isRegisteredSuccessfully != value)
            {
                _isRegisteredSuccessfully = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set
        {
            if (_isLoggedIn != value)
            {
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }
    }
    
    public string? CurrentUserId => _repository.GetCurrentUserId();
    public string? CurrentUserEmail => _repository.GetCurrentUserEmail();

    // Login method that updates IsLoggedIn
    public async Task LoginAsync(string email, string password)
    {
        await _repository.Login(email, password);
        IsLoggedIn = _repository.IsLoggedIn;
    }

    public async Task RegisterAsync(string email, string password, string username)
    {
        await _repository.Register(email, password, username);
        IsRegisteredSuccessfully = true;
    }

    public async Task LogoutAsync()
    {
        await _repository.Logout();
        IsLoggedIn = _repository.IsLoggedIn;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}