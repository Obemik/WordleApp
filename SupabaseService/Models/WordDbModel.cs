using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SupabaseService.Models;

[Table("words")]
public class WordDbModel : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    
    [Column("word")]
    public string Word { get; set; } = string.Empty;
    
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("added_by")]
    public string? AddedBy { get; set; }
}