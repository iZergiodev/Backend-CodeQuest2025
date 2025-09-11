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

}
