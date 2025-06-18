using System.Windows;
using System.Windows.Controls;
using WordleApp.ViewModels;
using WordleApp.Services;

namespace WordleApp.Views;

public partial class AuthenticationPage : UserControl
{
    private readonly NavigationService _navigationService;
    private readonly AuthenticationViewModel _viewModel;

    public AuthenticationPage(NavigationService navigationService, AuthenticationViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        _navigationService = navigationService;
        
        // Subscribe to PasswordBox changes
        PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
        
        // Subscribe to property changes to clear password box
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthenticationViewModel viewModel)
        {
            viewModel.Password = ((PasswordBox)sender).Password;
        }
    }
    
    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AuthenticationViewModel.Password) && string.IsNullOrEmpty(_viewModel.Password))
        {
            PasswordBox.Password = string.Empty;
        }
        
        // Show success messages
        if (e.PropertyName == nameof(AuthenticationViewModel.IsLoginSuccessful) && _viewModel.IsLoginSuccessful)
        {
            MessageBox.Show("Вхід успішний!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        if (e.PropertyName == nameof(AuthenticationViewModel.IsRegistrationSuccessful) && _viewModel.IsRegistrationSuccessful)
        {
            MessageBox.Show("Реєстрація успішна! Тепер увійдіть в систему.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    
    private void OnSubmitClick(object sender, RoutedEventArgs e)
    {
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
    }
}