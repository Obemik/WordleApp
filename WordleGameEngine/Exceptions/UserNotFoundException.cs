namespace WordleGameEngine.Exceptions;

/// <summary>
/// Виняток для користувачів
/// </summary>
public class UserNotFoundException : Exception
{
    public string UserId { get; }

    public UserNotFoundException(string userId) : base($"User not found: {userId}")
    {
        UserId = userId;
    }

    public UserNotFoundException(string userId, string message) : base(message)
    {
        UserId = userId;
    }

    public UserNotFoundException(string userId, string message, Exception innerException) : base(message, innerException)
    {
        UserId = userId;
    }
}