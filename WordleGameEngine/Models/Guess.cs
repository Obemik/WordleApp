using WordleGameEngine.Enums;

namespace WordleGameEngine.Models;
/// <summary>
/// Модель спроби відгадування
/// </summary>
public class Guess
{
    public string Word { get; set; } = string.Empty;
    public GuessResult[] Results { get; set; } = Array.Empty<GuessResult>();
    public DateTime Timestamp { get; set; }

    public Guess() { }

    public Guess(string word, GuessResult[] results)
    {
        Word = word.ToUpper();
        Results = results;
        Timestamp = DateTime.Now;
    }
}