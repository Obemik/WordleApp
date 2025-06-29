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
        Console.WriteLine($"[NavigationService] Navigating to {typeof(TView).Name}");
        
        var view = _serviceProvider.GetRequiredService<TView>();
        
        Console.WriteLine($"[NavigationService] View DataContext: {view.DataContext?.GetType().Name}");
        
        OnNavigate?.Invoke(view);
    }
}