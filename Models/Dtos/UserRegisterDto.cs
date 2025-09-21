using System;

namespace CodeQuestBackend.Models.Dtos;

public class UserRegisterDto
{
    public string? ID { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Username { get; set; }
    public string? Role { get; set; }
    public string? Avatar { get; set; }
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int StarDustPoints { get; set; }
}
