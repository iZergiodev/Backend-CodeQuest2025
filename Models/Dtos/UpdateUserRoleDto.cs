using System.ComponentModel.DataAnnotations;

namespace CodeQuestBackend.Models.Dtos;

public class UpdateUserRoleDto
{
    [Required]
    public string Role { get; set; } = string.Empty;
}
