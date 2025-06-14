using System;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace WordleApp.Services;

public class NavigationService
{
    private readonly IServiceProvider _serviceProvider;
    public event Action<UserControl>? OnNavigate;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<TView, TViewModel>() 
        where TView : UserControl
        where TViewModel : class
    {
        var view = _serviceProvider.GetRequiredService<TView>();
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        view.DataContext = viewModel;
        OnNavigate?.Invoke(view);
    }
}