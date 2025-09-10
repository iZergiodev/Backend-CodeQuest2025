using System;

namespace CodeQuestBackend.Models.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Avatar { get; set; }
    public string? Biography { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int StarDustPoints { get; set; }

}
