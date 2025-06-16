using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WordleApp.Models;
using WordleGameEngine.Enums;

namespace WordleApp.Views.Controls;

public partial class GameGrid : UserControl
{
    public static readonly DependencyProperty GameRowsProperty =
        DependencyProperty.Register("GameRows", typeof(ObservableCollection<GuessModel>), typeof(GameGrid), 
            new PropertyMetadata(new ObservableCollection<GuessModel>()));

    public ObservableCollection<GuessModel> GameRows
    {
        get { return (ObservableCollection<GuessModel>)GetValue(GameRowsProperty); }
        set { SetValue(GameRowsProperty, value); }
    }

    public GameGrid()
    {
        InitializeComponent();
        DataContext = this;
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        GameRows.Clear();
        
        // Створюємо 6 рядків для спроб
        for (int row = 0; row < 6; row++)
        {
            var guessModel = new GuessModel();
            
            // Кожний рядок має 5 літер
            for (int col = 0; col < 5; col++)
            {
                guessModel.Letters.Add(new LetterModel
                {
                    Letter = "",
                    Status = GuessResult.Absent
                });
            }
            
            GameRows.Add(guessModel);
        }
    }

    /// <summary>
    /// Оновлює рядок з результатами відгадування
    /// </summary>
    public void UpdateRow(int rowIndex, string word, GuessResult[] results)
    {
        if (rowIndex < 0 || rowIndex >= GameRows.Count) return;
        if (word.Length != 5 || results.Length != 5) return;

        var row = GameRows[rowIndex];
        
        for (int i = 0; i < 5; i++)
        {
            row.Letters[i].Letter = word[i].ToString().ToUpper();
            row.Letters[i].Status = results[i];
        }
    }

    /// <summary>
    /// Очищає всю сітку
    /// </summary>
    public void ClearGrid()
    {
        foreach (var row in GameRows)
        {
            foreach (var letter in row.Letters)
            {
                letter.Letter = "";
                letter.Status = GuessResult.Absent;
            }
        }
    }

    /// <summary>
    /// Оновлює поточний рядок з літерами
    /// </summary>
    public void UpdateCurrentRow(int rowIndex, string currentWord)
    {
        if (rowIndex < 0 || rowIndex >= GameRows.Count) return;

        var row = GameRows[rowIndex];
        
        for (int i = 0; i < 5; i++)
        {
            if (i < currentWord.Length)
            {
                row.Letters[i].Letter = currentWord[i].ToString().ToUpper();
            }
            else
            {
                row.Letters[i].Letter = "";
            }
            
            // Зберігаємо статус для вже відгаданих рядків
            if (rowIndex > 0 && !string.IsNullOrEmpty(row.Letters[i].Letter))
            {
                // Не змінюємо статус для вже заповнених рядків
            }
            else
            {
                row.Letters[i].Status = GuessResult.Absent;
            }
        }
    }

    /// <summary>
    /// Отримує поточний стан сітки як рядки
    /// </summary>
    public List<string> GetGridState()
    {
        var state = new List<string>();
        
        foreach (var row in GameRows)
        {
            var rowString = "";
            foreach (var letter in row.Letters)
            {
                rowString += letter.Letter;
            }
            state.Add(rowString);
        }
        
        return state;
    }

    /// <summary>
    /// Встановлює стан сітки з рядків
    /// </summary>
    public void SetGridState(List<string> words, List<GuessResult[]> results)
    {
        ClearGrid();
        
        for (int i = 0; i < Math.Min(words.Count, results.Count) && i < GameRows.Count; i++)
        {
            if (!string.IsNullOrEmpty(words[i]) && results[i] != null)
            {
                UpdateRow(i, words[i], results[i]);
            }
        }
    }
}