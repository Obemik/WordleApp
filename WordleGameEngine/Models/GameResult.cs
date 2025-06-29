using WordleGameEngine.Enums;

namespace WordleGameEngine.Models;

public class GameResult
{
    public bool IsWon { get; set; }
    public int AttemptsUsed { get; set; }
    public string TargetWord { get; set; } = string.Empty;
    public List<Guess> Guesses { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public DateTime CompletedAt { get; set; }

    public GameResult()
    {
        CompletedAt = DateTime.Now;
    }

    public GameResult(Game game)
    {
        IsWon = game.IsWon;
        AttemptsUsed = game.Attempts;
        TargetWord = game.TargetWord;
        Guesses = new List<Guess>(game.Guesses);
        CompletedAt = DateTime.Now;

        if (game.CompletedAt.HasValue)
        {
            Duration = game.CompletedAt.Value - game.StartedAt;
        }
    }
}    