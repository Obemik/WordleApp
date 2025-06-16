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
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthenticationViewModel viewModel)
        {
            viewModel.Password = ((PasswordBox)sender).Password;
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
}