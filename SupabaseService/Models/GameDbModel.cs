using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SupabaseService.Models;

[Table("games")]
public class GameDbModel : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;
    
    [Column("target_word")]
    public string TargetWord { get; set; } = string.Empty;
    
    [Column("guesses")]
    public string Guesses { get; set; } = string.Empty; // JSON string of guesses
    
    [Column("is_won")]
    public bool IsWon { get; set; }
    
    [Column("attempts_count")]
    public int AttemptsCount { get; set; }
    
    [Column("game_status")]
    public string GameStatus { get; set; } = "InProgress"; // InProgress, Won, Lost
    
    [Column("started_at")]
    public DateTime StartedAt { get; set; }
    
    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }
}
