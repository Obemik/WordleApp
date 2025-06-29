using WordleGameEngine.Enums;

namespace WordleGameEngine.Services;

public class GuessValidatorService
{
    /// <summary>
    /// Перевіряє спробу відгадування та повертає результати для кожної літери
    /// </summary>
    public GuessResult[] ValidateGuess(string targetWord, string guessWord)
    {
        if (targetWord.Length != guessWord.Length)
            throw new ArgumentException("Target word and guess word must have the same length");

        var results = new GuessResult[guessWord.Length];
        var targetChars = targetWord.ToCharArray();
        var guessChars = guessWord.ToUpper().ToCharArray();
        
        // Спочатку позначаємо всі точні збіги
        for (int i = 0; i < guessChars.Length; i++)
        {
            if (guessChars[i] == targetChars[i])
            {
                results[i] = GuessResult.Correct;
                targetChars[i] = '*'; // Позначаємо як використану
                guessChars[i] = '*';  // Позначаємо як перевірену
            }
        }

        // Потім перевіряємо частково правильні літери
        for (int i = 0; i < guessChars.Length; i++)
        {
            if (guessChars[i] == '*') continue; // Вже перевірена як точний збіг

            bool found = false;
            for (int j = 0; j < targetChars.Length; j++)
            {
                if (targetChars[j] == guessChars[i])
                {
                    results[i] = GuessResult.Present;
                    targetChars[j] = '*'; // Позначаємо як використану
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                results[i] = GuessResult.Absent;
            }
        }

        return results;
    }

    /// <summary>
    /// Перевіряє, чи є слово повністю правильним
    /// </summary>
    public bool IsWordCorrect(GuessResult[] results)
    {
        return results.All(r => r == GuessResult.Correct);
    }

    /// <summary>
    /// Підраховує кількість правильних літер на правильних позиціях
    /// </summary>
    public int GetCorrectCount(GuessResult[] results)
    {
        return results.Count(r => r == GuessResult.Correct);
    }

    /// <summary>
    /// Підраховує кількість правильних літер на неправильних позиціях
    /// </summary>
    public int GetPresentCount(GuessResult[] results)
    {
        return results.Count(r => r == GuessResult.Present);
    }

    /// <summary>
    /// Підраховує кількість відсутніх літер
    /// </summary>
    public int GetAbsentCount(GuessResult[] results)
    {
        return results.Count(r => r == GuessResult.Absent);
    }
}