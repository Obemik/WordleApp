using System.Windows;
using System.Windows.Controls;
using WordleApp.ViewModels;
using WordleApp.Services;

namespace WordleApp.Views;

public partial class AuthenticationPage : UserControl
{
    private readonly NavigationService _navigationService;
    private AuthenticationViewModel _viewModel => (AuthenticationViewModel)DataContext;

    public AuthenticationPage(NavigationService navigationService, AuthenticationViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
        _navigationService = navigationService;
        
        // Subscribe to PasswordBox changes
        PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
        
        // Subscribe to property changes ПІСЛЯ встановлення DataContext
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
        
        Loaded += (s, e) => 
        {
            EmailTextBox.Focus();
            Console.WriteLine($"[AuthenticationPage.Loaded] DataContext type: {DataContext?.GetType().Name}");
            Console.WriteLine($"[AuthenticationPage.Loaded] ViewModel Email: '{_viewModel?.Email}'");
        };
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.Password = ((PasswordBox)sender).Password;
        }
    }
    
    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AuthenticationViewModel.Password) && string.IsNullOrEmpty(_viewModel?.Password))
        {
            PasswordBox.Password = string.Empty;
        }
        
        if (e.PropertyName == nameof(AuthenticationViewModel.IsRegistrationSuccessful) && _viewModel?.IsRegistrationSuccessful == true)
        {
            MessageBox.Show("Реєстрація успішна! Тепер увійдіть в систему.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    
    private void OnSubmitClick(object sender, RoutedEventArgs e)
    {
        // Діагностика
        Console.WriteLine($"[AuthenticationPage.OnSubmitClick] DataContext: {DataContext?.GetType().Name}");
        Console.WriteLine($"[AuthenticationPage.OnSubmitClick] Email: '{_viewModel?.Email}', Password length: {_viewModel?.Password?.Length ?? 0}");
        
        if (_viewModel == null)
        {
            Console.WriteLine("[AuthenticationPage.OnSubmitClick] ERROR: ViewModel is null!");
            return;
        }
        
        if (_viewModel.IsLoginMode)
        {
            _viewModel.LoginCommand.Execute(null);
        }
        else
        {
            _viewModel.RegisterCommand.Execute(null);
        }
    }
    
    private void OnSwitchModeClick(object sender, RoutedEventArgs e)
    {
        // Clear the password box when switching modes
        PasswordBox.Password = string.Empty;
        EmailTextBox.Focus();
    }
}