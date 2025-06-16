using System.Windows;
using System.Windows.Controls;
using WordleApp.ViewModels;
using WordleApp.Services;

namespace WordleApp.Views;

public partial class AuthenticationPage : UserControl
{
    private readonly NavigationService _navigationService;

    public AuthenticationPage(NavigationService navigationService, AuthenticationViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _navigationService = navigationService;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthenticationViewModel viewModel)
        {
            viewModel.Password = ((PasswordBox)sender).Password;
        }
    }
}