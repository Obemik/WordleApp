namespace WordleGameEngine.Exceptions;

/// <summary>
/// Виняток для невалідних слів
/// </summary>
public class InvalidWordException : Exception
{
    public string Word { get; }

    public InvalidWordException(string word) : base($"Invalid word: {word}")
    {
        Word = word;
    }

    public InvalidWordException(string word, string message) : base(message)
    {
        Word = word;
    }

    public InvalidWordException(string word, string message, Exception innerException) : base(message, innerException)
    {
        Word = word;
    }
}