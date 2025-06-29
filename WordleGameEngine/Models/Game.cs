using WordleGameEngine.Enums;

namespace WordleGameEngine.Models;

public class Game
{
    public string TargetWord { get; set; } = string.Empty;
    public List<Guess> Guesses { get; set; } = new();
    public GameStatus Status { get; set; } = GameStatus.InProgress;
    public int Attempts { get; set; } = 0;
    public int MaxAttempts { get; set; } = 6;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Game() 
    {
        StartedAt = DateTime.Now;
    }

    public Game(string targetWord, int maxAttempts = 6)
    {
        TargetWord = targetWord.ToUpper();
        MaxAttempts = maxAttempts;
        StartedAt = DateTime.Now;
    }

    public bool IsGameOver => Status != GameStatus.InProgress;
    public bool IsWon => Status == GameStatus.Won;
    public bool IsLost => Status == GameStatus.Lost;
}