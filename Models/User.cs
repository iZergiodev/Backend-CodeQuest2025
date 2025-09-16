using System;
using System.ComponentModel.DataAnnotations;

namespace CodeQuestBackend.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? Avatar { get; set; }
    public string? Biography { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int StarDustPoints { get; set; }
    
    // Discord authentication fields
    public string? DiscordId { get; set; }
    public string? DiscordUsername { get; set; }
    public string? DiscordDiscriminator { get; set; }
    public string? DiscordAvatar { get; set; }
    public string? DiscordAccessToken { get; set; }
    public string? DiscordRefreshToken { get; set; }
    public DateTime? DiscordTokenExpiresAt { get; set; }
    
    // Navigation Properties for following subcategories
    public virtual ICollection<UserSubcategoryFollow> FollowedSubcategories { get; set; } = new List<UserSubcategoryFollow>();
}
