using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
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
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(NavigationService navigationService, AuthenticationService authService, 
        SupabaseRepository repository, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _navigationService = navigationService;
        _authService = authService;
        _repository = repository;
        _serviceProvider = serviceProvider;
    
        _navigationService.OnNavigate += SetContent;
        _authService.PropertyChanged += OnAuthServicePropertyChanged!;
    }
    private async void OnAuthServicePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        Console.WriteLine($"[MainWindow] Property changed: {e.PropertyName}, IsLoggedIn: {_authService.IsLoggedIn}");
        if (e.PropertyName == nameof(AuthenticationService.IsLoggedIn))
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                var gameService = _serviceProvider.GetRequiredService<GameService>();
                gameService.ClearCache();
        
                if (_authService.IsLoggedIn)
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
                    else
                    {
                        _navigationService.NavigateTo<PlayerMenuPage, PlayerMenuViewModel>();
                    }
                }
                else
                {
                    _navigationService.NavigateTo<AuthenticationPage, AuthenticationViewModel>();
                }
            });
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