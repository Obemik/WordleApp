using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordleApp.ViewModels;
using WordleApp.Services;

namespace WordleApp.Views;

public partial class AdminPanelPage : UserControl
{
    private readonly NavigationService _navigationService;

    public AdminPanelPage(NavigationService navigationService, AdminPanelViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _navigationService = navigationService;
    }

    private void OnLogoutClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Ви впевнені, що хочете вийти?", "Підтвердження", 
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            if (DataContext is AdminPanelViewModel viewModel)
            {
                viewModel.LogoutCommand.Execute(null);
            }
            _navigationService.NavigateTo<AuthenticationPage, AuthenticationViewModel>();
        }
    }

    private void OnAddWordKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is AdminPanelViewModel viewModel)
        {
            viewModel.AddWordCommand.Execute(null);
        }
    }
}