using WordleGameEngine.Enums;

namespace WordleGameEngine.Models;

public class Player
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Player;
    public DateTime CreatedAt { get; set; }

    public Player() 
    {
        CreatedAt = DateTime.Now;
    }

    public Player(string id, string username, string email, UserRole role = UserRole.Player)
    {
        Id = id;
        Username = username;
        Email = email;
        Role = role;
        CreatedAt = DateTime.Now;
    }
}