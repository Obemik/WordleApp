using System.Windows.Input;
using WordleApp.ViewModels;
using WordleApp.Services;
using WordleApp.Helpers;
using WordleApp.Views;

namespace WordleApp.ViewModels;

public class AuthenticationViewModel : BaseViewModel
{
    private readonly AuthenticationService _authService;
    private readonly NavigationService _navigationService;
    
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _username = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoginMode = true;
    private bool _isLoading = false;

    public AuthenticationViewModel(AuthenticationService authService, NavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;

        LoginCommand = new RelayCommand(async () => await LoginAsync(), CanLogin);
        RegisterCommand = new RelayCommand(async () => await RegisterAsync(), CanRegister);
        SwitchModeCommand = new RelayCommand(SwitchMode);

        // Subscribe to authentication service changes
        _authService.PropertyChanged += OnAuthServicePropertyChanged;
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoginMode
    {
        get => _isLoginMode;
        set => SetProperty(ref _isLoginMode, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string ModeText => IsLoginMode ? "Увійти" : "Зареєструватися";
    public string SwitchModeText => IsLoginMode ? "Немає акаунту? Зареєструватися" : "Вже є акаунт? Увійти";

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }
    public ICommand SwitchModeCommand { get; }

    private async Task LoginAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            await _authService.LoginAsync(Email, Password);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Помилка входу: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RegisterAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            await _authService.RegisterAsync(Email, Password, Username);
            
            if (_authService.IsRegisteredSuccessfully)
            {
                ErrorMessage = "Реєстрація успішна! Тепер увійдіть в систему.";
                IsLoginMode = true;
                ClearFields();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Помилка реєстрації: {ex.Message}";
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
        ClearFields();
    }

    private void ClearFields()
    {
        Email = string.Empty;
        Password = string.Empty;
        Username = string.Empty;
    }

    private bool CanLogin()
    {
        return !IsLoading && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
    }

    private bool CanRegister()
    {
        return !IsLoading && !string.IsNullOrWhiteSpace(Email) && 
               !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Username);
    }

    private void OnAuthServicePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AuthenticationService.IsLoggedIn) && _authService.IsLoggedIn)
        {
            // Navigate to main menu when successfully logged in
            _navigationService.NavigateTo<PlayerMenuPage, PlayerMenuViewModel>();
        }
    }
}