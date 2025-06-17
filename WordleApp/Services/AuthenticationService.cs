using System.ComponentModel;
using System.Runtime.CompilerServices;
using SupabaseService.Repository;
using SupabaseService.Models;

namespace WordleApp.Services;

public class AuthenticationService : INotifyPropertyChanged
{
    private readonly SupabaseRepository _repository;
    private bool _isLoggedIn;
    private bool _isRegisteredSuccessfully = false;
    private bool _isInitialized = false;
    private UserDbModel? _currentUser;

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
        
        // Load current user if logged in
        if (IsLoggedIn && !string.IsNullOrEmpty(CurrentUserId))
        {
            await LoadCurrentUserAsync();
        }
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
    
    public UserDbModel? CurrentUser
    {
        get => _currentUser;
        private set
        {
            if (_currentUser != value)
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }
    }
    
    public string? CurrentUserId => _repository.GetCurrentUserId();
    public string? CurrentUserEmail => _repository.GetCurrentUserEmail();
    public string? CurrentUsername => _currentUser?.Username;
    public string? CurrentUserRole => _currentUser?.Role;

    // Login method that updates IsLoggedIn
    public async Task LoginAsync(string email, string password)
    {
        await _repository.Login(email, password);
        IsLoggedIn = _repository.IsLoggedIn;
        
        if (IsLoggedIn)
        {
            await LoadCurrentUserAsync();
        }
    }

    public async Task RegisterAsync(string email, string password, string username)
    {
        await _repository.Register(email, password, username);
        
        // After registration, create user record in users table
        var userId = _repository.GetCurrentUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            await _repository.CreateUserAsync(userId, username, email);
        }
        
        IsRegisteredSuccessfully = true;
    }

    public async Task LogoutAsync()
    {
        await _repository.Logout();
        IsLoggedIn = _repository.IsLoggedIn;
        CurrentUser = null;
    }
    
    private async Task LoadCurrentUserAsync()
    {
        var userId = CurrentUserId;
        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                CurrentUser = await _repository.GetUserByIdAsync(userId);
            }
            catch
            {
                // User might not exist in users table yet
                CurrentUser = null;
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}