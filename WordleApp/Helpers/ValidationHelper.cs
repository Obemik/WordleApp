using System.Text.RegularExpressions;

namespace WordleApp.Helpers;

public static class ValidationHelper
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            return emailRegex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return password.Length >= 6;
    }

    public static bool IsValidUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        var usernameRegex = new Regex(@"^[a-zA-Z0-9_]{3,20}$");
        return usernameRegex.IsMatch(username);
    }

    public static bool IsValidWordleWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return false;

        var wordRegex = new Regex(@"^[a-zA-Z]{5}$");
        return wordRegex.IsMatch(word);
    }

    public static string CleanString(string input)
    {
        return string.IsNullOrWhiteSpace(input) ? string.Empty : input.Trim();
    }

    public static bool IsOnlyLetters(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        return input.All(char.IsLetter);
    }

    public static bool IsOnlyDigits(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        return input.All(char.IsDigit);
    }

    public static bool IsSqlSafe(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return true;

        var sqlKeywords = new[] { "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER", "EXEC", "UNION", "SCRIPT" };
        var upperInput = input.ToUpper();

        return !sqlKeywords.Any(keyword => upperInput.Contains(keyword));
    }

    public static string LimitLength(string input, int maxLength)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return input.Length <= maxLength ? input : input[..maxLength];
    }
}