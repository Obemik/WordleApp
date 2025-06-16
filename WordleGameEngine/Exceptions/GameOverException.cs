namespace WordleGameEngine.Exceptions;

/// <summary>
/// Виняток для завершення гри
/// </summary>
public class GameOverException : Exception
{
    public string GameStatus { get; }

    public GameOverException(string gameStatus) : base($"Game is over with status: {gameStatus}")
    {
        GameStatus = gameStatus;
    }

    public GameOverException(string gameStatus, string message) : base(message)
    {
        GameStatus = gameStatus;
    }

    public GameOverException(string gameStatus, string message, Exception innerException) : base(message, innerException)
    {
        GameStatus = gameStatus;
    }
}