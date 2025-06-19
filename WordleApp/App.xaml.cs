using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WordleApp.Services;
using WordleApp.ViewModels;
using WordleApp.Views;
using SupabaseService.Repository;
using WordleGameEngine.Services;

namespace WordleApp;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;
    public IServiceProvider Services => _serviceProvider!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Configure Logging
        services.AddLogging();

        // Register SupabaseService layer
        services.AddSingleton<SupabaseRepository>();
        
        // Register WordleGameEngine services
        services.AddSingleton<GameEngineService>();
        services.AddSingleton<GuessValidatorService>();
        services.AddSingleton<WordGeneratorService>();
        
        // Register NavigationService
        services.AddSingleton<NavigationService>();
        
        // Register application services
        services.AddSingleton<AuthenticationService>(sp =>
        {
            var repository = sp.GetRequiredService<SupabaseRepository>();
            return new AuthenticationService(repository);
        });
        services.AddSingleton<GameService>(sp =>
        {
            var repository = sp.GetRequiredService<SupabaseRepository>();
            var gameEngine = sp.GetRequiredService<GameEngineService>();
            var authService = sp.GetRequiredService<AuthenticationService>();
            var wordValidationService = sp.GetRequiredService<WordValidationService>();
            return new GameService(repository, gameEngine, authService, wordValidationService);
        });
        services.AddSingleton<WordValidationService>();
        
        // Register ViewModels
        services.AddSingleton<AuthenticationViewModel>();
        services.AddSingleton<PlayerMenuViewModel>();
        services.AddSingleton<GamePageViewModel>();
        services.AddSingleton<AdminPanelViewModel>();
        
        // Register Views
        services.AddSingleton<AuthenticationPage>(sp =>
        {
            var navigationService = sp.GetRequiredService<NavigationService>();
            var viewModel = sp.GetRequiredService<AuthenticationViewModel>();
            return new AuthenticationPage(navigationService, viewModel);
        });
        services.AddSingleton<PlayerMenuPage>(sp =>
        {
            var navigationService = sp.GetRequiredService<NavigationService>();
            var viewModel = sp.GetRequiredService<PlayerMenuViewModel>();
            return new PlayerMenuPage(navigationService, viewModel);
        });
        services.AddSingleton<GamePage>(sp =>
        {
            var navigationService = sp.GetRequiredService<NavigationService>();
            var viewModel = sp.GetRequiredService<GamePageViewModel>();
            return new GamePage(navigationService, viewModel);
        });
        services.AddSingleton<AdminPanelPage>(sp =>
        {
            var navigationService = sp.GetRequiredService<NavigationService>();
            var viewModel = sp.GetRequiredService<AdminPanelViewModel>();
            return new AdminPanelPage(navigationService, viewModel);
        });
        
        // Register MainWindow
        services.AddSingleton<MainWindow>(sp =>
        {
            var navigationService = sp.GetRequiredService<NavigationService>();
            var authService = sp.GetRequiredService<AuthenticationService>();
            var repository = sp.GetRequiredService<SupabaseRepository>();
            return new MainWindow(navigationService, authService, repository);
        });
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
    
}