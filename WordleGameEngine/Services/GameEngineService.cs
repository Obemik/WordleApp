using WordleGameEngine.Models;
using WordleGameEngine.Enums;

namespace WordleGameEngine.Services;

public class GameEngineService
{
    private readonly GuessValidatorService _guessValidator;
    private readonly WordGeneratorService _wordGenerator;

    public GameEngineService()
    {
        _guessValidator = new GuessValidatorService();
        _wordGenerator = new WordGeneratorService();
    }

    /// <summary>
    /// Створює нову гру з заданим словом
    /// </summary>
    public Game CreateGame(string targetWord, int maxAttempts = 6)
    {
        if (string.IsNullOrWhiteSpace(targetWord) || targetWord.Length != 5)
            throw new ArgumentException("Target word must be exactly 5 characters long");

        return new Game(targetWord, maxAttempts);
    }

    /// <summary>
    /// Робить спробу відгадування
    /// </summary>
    public GuessResult[] MakeGuess(Game game, string guessWord)
    {
        if (game.IsGameOver)
            throw new InvalidOperationException("Game is already finished");

        if (!IsValidWord(guessWord))
            throw new ArgumentException("Invalid word format");

        var results = _guessValidator.ValidateGuess(game.TargetWord, guessWord);
        var guess = new Guess(guessWord, results);
        
        game.Guesses.Add(guess);
        game.Attempts++;

        // Check if word is guessed correctly
        if (results.All(r => r == GuessResult.Correct))
        {
            game.Status = GameStatus.Won;
            game.CompletedAt = DateTime.Now;
        }
        // Check if max attempts reached
        else if (game.Attempts >= game.MaxAttempts)
        {
            game.Status = GameStatus.Lost;
            game.CompletedAt = DateTime.Now;
        }

        return results;
    }

    /// <summary>
    /// Перевіряє, чи є слово валідним для гри
    /// </summary>
    public bool IsValidWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return false;

        if (word.Length != 5)
            return false;

        return word.All(char.IsLetter);
    }

    /// <summary>
    /// Отримує стан гри
    /// </summary>
    public GameResult GetGameResult(Game game)
    {
        return new GameResult(game);
    }

    /// <summary>
    /// Перевіряє, чи можна зробити ще одну спробу
    /// </summary>
    public bool CanMakeGuess(Game game)
    {
        return !game.IsGameOver && game.Attempts < game.MaxAttempts;
    }
}