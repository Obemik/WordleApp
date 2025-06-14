namespace WordleGameEngine.Enums;

public enum GuessResult
{
    /// <summary>
    /// Літера відсутня у слові
    /// </summary>
    Absent = 0,
    
    /// <summary>
    /// Літера присутня у слові, але не на цій позиції
    /// </summary>
    Present = 1,
    
    /// <summary>
    /// Літера присутня у слові на правильній позиції
    /// </summary>
    Correct = 2
}