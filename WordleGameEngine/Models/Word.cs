using WordleGameEngine.Enums;

namespace WordleGameEngine.Models;

/// <summary>
/// Базова модель слова
/// </summary>
public class Word
{
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public Word() { }

    public Word(string value)
    {
        Value = value.ToUpper();
        CreatedAt = DateTime.Now;
    }
}