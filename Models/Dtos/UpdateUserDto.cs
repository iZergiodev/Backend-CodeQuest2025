using System.ComponentModel.DataAnnotations;

namespace CodeQuestBackend.Models.Dtos;

public class UpdateUserDto
{
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; }

    [StringLength(500, ErrorMessage = "Biography cannot exceed 500 characters")]
    public string? Biography { get; set; }

    public string? BirthDate { get; set; }

    [StringLength(500, ErrorMessage = "Avatar URL cannot exceed 500 characters")]
    public string? Avatar { get; set; }
}
