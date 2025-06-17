using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WordleApp.Services;
using WordleApp.ViewModels;
using WordleApp.Views;
using SupabaseService.Repository;

namespace WordleApp;

public partial class MainWindow : Window
{
    private readonly NavigationService _navigationService;
    private readonly AuthenticationService _authService;
    private readonly SupabaseRepository _repository;

    public MainWindow(NavigationService navigationService, AuthenticationService authService, SupabaseRepository repository)
    {
        InitializeComponent();
        _navigationService = navigationService;
        _authService = authService;
        _repository = repository;
        
        _navigationService.OnNavigate += SetContent;
        _authService.PropertyChanged += OnAuthServicePropertyChanged!;
    }

    private async void OnAuthServicePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AuthenticationService.IsLoggedIn) && _authService.IsLoggedIn)
        {
            // Check user role
            var userId = _authService.CurrentUserId;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _repository.GetUserByIdAsync(userId);
                if (user != null && user.Role == "Admin")
                {
                    _navigationService.NavigateTo<AdminPanelPage, AdminPanelViewModel>();
                }
                else
                {
                    _navigationService.NavigateTo<PlayerMenuPage, PlayerMenuViewModel>();
                }
            }
        }
        
        if (e.PropertyName == nameof(AuthenticationService.IsLoggedIn) && !_authService.IsLoggedIn)
        {
            _navigationService.NavigateTo<AuthenticationPage, AuthenticationViewModel>();
        }
        
        if (e.PropertyName == nameof(AuthenticationService.IsRegisteredSuccessfully) && _authService.IsRegisteredSuccessfully)
        {
            _authService.IsRegisteredSuccessfully = false;
            _navigationService.NavigateTo<AuthenticationPage, AuthenticationViewModel>();
        }
    }

    private void SetContent(UserControl content)
    {
        this.Content = content;
    }

    private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_authService.IsLoggedIn)
        {
            var userId = _authService.CurrentUserId;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _repository.GetUserByIdAsync(userId);
                if (user != null && user.Role == "Admin")
                {
                    _navigationService.NavigateTo<AdminPanelPage, AdminPanelViewModel>();
                }
                else
                {
                    _navigationService.NavigateTo<PlayerMenuPage, PlayerMenuViewModel>();
                }
            }
        }
        else
        {
            _navigationService.NavigateTo<AuthenticationPage, AuthenticationViewModel>();
        }
    }
}