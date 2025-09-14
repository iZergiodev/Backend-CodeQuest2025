using System.ComponentModel.DataAnnotations;

namespace CodeQuestBackend.Models.Dtos;

public class UpdateCategoryDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Color { get; set; }
}