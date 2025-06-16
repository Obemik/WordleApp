using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WordleApp.Services;
using WordleApp.ViewModels;
using WordleApp.Views;

namespace WordleApp;

public partial class MainWindow : Window
{
    private readonly NavigationService _navigationService;
    private readonly AuthenticationService _authService;

    public MainWindow(NavigationService navigationService, AuthenticationService authService)
    {
        InitializeComponent();
        _navigationService = navigationService;
        _authService = authService;
        
        _navigationService.OnNavigate += SetContent;
        _authService.PropertyChanged += OnAuthServicePropertyChanged!;
    }

    private void OnAuthServicePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AuthenticationService.IsLoggedIn) && _authService.IsLoggedIn)
        {
            // TODO: Check user role and navigate to appropriate page
            _navigationService.NavigateTo<PlayerMenuPage, PlayerMenuViewModel>();
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

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_authService.IsLoggedIn)
            _navigationService.NavigateTo<PlayerMenuPage, PlayerMenuViewModel>();
        else
            _navigationService.NavigateTo<AuthenticationPage, AuthenticationViewModel>();
    }
}