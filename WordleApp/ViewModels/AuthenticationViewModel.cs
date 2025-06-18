using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WordleApp.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace WordleApp.ViewModels;

public class AuthenticationViewModel : INotifyPropertyChanged
{
    private readonly AuthenticationService _authService;
    private readonly ILogger<AuthenticationViewModel>? _logger;
    
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _username = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoginMode = true;
    private bool _isLoading = false;
    private bool _isLoginSuccessful = false;
    private bool _isRegistrationSuccessful = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    public AuthenticationViewModel(AuthenticationService authService, ILogger<AuthenticationViewModel>? logger = null)
    {
        _authService = authService;
        _logger = logger;

        LoginCommand = new AsyncRelayCommand(OnLoginAsync);
        RegisterCommand = new AsyncRelayCommand(OnRegisterAsync);
        SwitchModeCommand = new RelayCommand(SwitchMode);
    }

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public bool IsLoginMode
    {
        get => _isLoginMode;
        set
        {
            _isLoginMode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ModeText));
            OnPropertyChanged(nameof(SwitchModeText));
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public bool IsLoginSuccessful
    {
        get => _isLoginSuccessful;
        set
        {
            _isLoginSuccessful = value;
            OnPropertyChanged();
        }
    }

    public bool IsRegistrationSuccessful
    {
        get => _isRegistrationSuccessful;
        set
        {
            _isRegistrationSuccessful = value;
            OnPropertyChanged();
        }
    }

    public string ModeText => IsLoginMode ? "Увійти" : "Зареєструватися";
    public string SwitchModeText => IsLoginMode ? "Немає акаунту? Зареєструватися" : "Вже є акаунт? Увійти";

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }
    public ICommand SwitchModeCommand { get; }

    private async Task OnLoginAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            IsLoginSuccessful = false;

            await _authService.LoginAsync(Email, Password);
            IsLoginSuccessful = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Помилка входу: {ex.Message}";
            IsLoginSuccessful = false;
            _logger?.LogError(ex, "Login failed for user {Email}", Email);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task OnRegisterAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            IsRegistrationSuccessful = false;

            await _authService.RegisterAsync(Email, Password, Username);
            
            if (_authService.IsRegisteredSuccessfully)
            {
                ErrorMessage = string.Empty; // Clear any error messages
                IsRegistrationSuccessful = true;
                IsLoginMode = true;
                ClearFields();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Помилка реєстрації: {ex.Message}";
            IsRegistrationSuccessful = false;
            _logger?.LogError(ex, "Registration failed for user {Username}", Username);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SwitchMode()
    {
        IsLoginMode = !IsLoginMode;
        ErrorMessage = string.Empty;
        IsLoginSuccessful = false;
        IsRegistrationSuccessful = false;
        ClearFields();
    }

    private void ClearFields()
    {
        Email = string.Empty;
        Password = string.Empty;
        Username = string.Empty;
    }
    
    public void ClearPassword()
    {
        Password = string.Empty;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}