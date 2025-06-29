using System.Windows.Input;
using System.Collections.ObjectModel;
using WordleApp.ViewModels;
using WordleApp.Services;
using WordleApp.Models;
using SupabaseService.Repository;
using SupabaseService.Models;
using CommunityToolkit.Mvvm.Input;

namespace WordleApp.ViewModels;

public class AdminPanelViewModel : BaseViewModel
{
    private readonly SupabaseRepository _repository;
    private readonly AuthenticationService _authService;
    private readonly NavigationService _navigationService;
    private readonly WordValidationService _wordValidationService;
    
    private string _newWord = string.Empty;
    private string _searchText = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isLoading = false;

    public AdminPanelViewModel(SupabaseRepository repository, AuthenticationService authService, 
        NavigationService navigationService, WordValidationService wordValidationService)
    {
        _repository = repository;
        _authService = authService;
        _navigationService = navigationService;
        _wordValidationService = wordValidationService;

        Words = new ObservableCollection<WordModel>();
        FilteredWords = new ObservableCollection<WordModel>();

        AddWordCommand = new AsyncRelayCommand(AddWordAsync, CanAddWord);
        DeleteWordCommand = new AsyncRelayCommand<WordModel>(DeleteWordAsync);
        SearchCommand = new RelayCommand(SearchWords);
        RefreshCommand = new AsyncRelayCommand(LoadWordsAsync);
        LogoutCommand = new AsyncRelayCommand(LogoutAsync);

        LoadWordsAsync();
    }

    public ObservableCollection<WordModel> Words { get; }
    public ObservableCollection<WordModel> FilteredWords { get; }

    public string NewWord
    {
        get => _newWord;
        set 
        { 
            if (SetProperty(ref _newWord, value))
            {
                ((AsyncRelayCommand)AddWordCommand).NotifyCanExecuteChanged();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set 
        { 
            if (SetProperty(ref _searchText, value))
            {
                SearchWords();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string AdminEmail => _authService.CurrentUserEmail ?? "Адміністратор";

    public ICommand AddWordCommand { get; }
    public ICommand DeleteWordCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand LogoutCommand { get; }

    private async Task LoadWordsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Завантаження слів...";

            var wordsFromDb = await _repository.GetAllWordsAsync();
            
            Words.Clear();
            foreach (var wordDb in wordsFromDb)
            {
                Words.Add(new WordModel
                {
                    Id = wordDb.Id,
                    Word = wordDb.Word,
                    IsActive = wordDb.IsActive,
                    CreatedAt = wordDb.CreatedAt,
                    AddedBy = wordDb.AddedBy
                });
            }

            SearchWords();
            StatusMessage = $"Завантажено {Words.Count} слів";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Помилка завантаження: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task AddWordAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Додавання слова...";

            var cleanWord = NewWord.Trim().ToUpper();
            
            if (!_wordValidationService.IsValidWordFormat(cleanWord))
            {
                StatusMessage = "Слово повинно містити рівно 5 літер";
                return;
            }

            if (await _repository.WordExistsAsync(cleanWord))
            {
                StatusMessage = "Це слово вже існує в словнику";
                return;
            }

            var addedWord = await _repository.AddWordAsync(cleanWord, _authService.CurrentUserId);
            
            Words.Add(new WordModel
            {
                Id = addedWord.Id,
                Word = addedWord.Word,
                IsActive = addedWord.IsActive,
                CreatedAt = addedWord.CreatedAt,
                AddedBy = addedWord.AddedBy
            });

            NewWord = string.Empty;
            SearchWords();
            StatusMessage = $"Слово '{cleanWord}' успішно додано";
            
            // Notify that can add word command state changed
            ((AsyncRelayCommand)AddWordCommand).NotifyCanExecuteChanged();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Помилка додавання: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteWordAsync(WordModel? wordModel)
    {
        if (wordModel == null) return;

        try
        {
            IsLoading = true;
            StatusMessage = "Видалення слова...";

            var success = await _repository.DeleteWordAsync(wordModel.Id);
            
            if (success)
            {
                Words.Remove(wordModel);
                SearchWords();
                StatusMessage = "Слово успішно видалено";
            }
            else
            {
                StatusMessage = "Помилка видалення слова";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Помилка видалення: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SearchWords()
    {
        FilteredWords.Clear();
        
        var filtered = string.IsNullOrWhiteSpace(SearchText) 
            ? Words 
            : Words.Where(w => w.Word.Contains(SearchText.ToUpper(), StringComparison.OrdinalIgnoreCase));

        foreach (var word in filtered)
        {
            FilteredWords.Add(word);
        }
    }

    private async Task LogoutAsync()
    {
        try
        {
            await _authService.LogoutAsync();
            // Navigation will be handled in code-behind
        }
        catch (Exception ex)
        {
            StatusMessage = $"Помилка виходу: {ex.Message}";
        }
    }

    private bool CanAddWord()
    {
        return !IsLoading && !string.IsNullOrWhiteSpace(NewWord) && NewWord.Length == 5;
    }
    
    
}