using System.ComponentModel.DataAnnotations;

namespace CodeQuestBackend.Models.Dtos;

public class CreateSubcategoryDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Description { get; set; }

    [MaxLength(20, ErrorMessage = "El color no puede exceder 20 caracteres")]
    public string? Color { get; set; }

    [Required(ErrorMessage = "La categoría es requerida")]
    public int CategoryId { get; set; }
}