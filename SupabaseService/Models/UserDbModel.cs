using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SupabaseService.Models;

[Table("users")]
public class UserDbModel : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;
    
    [Column("username")]
    public string Username { get; set; } = string.Empty;
    
    [Column("email")]
    public string Email { get; set; } = string.Empty;
    
    [Column("role")]
    public string Role { get; set; } = "Player";
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}