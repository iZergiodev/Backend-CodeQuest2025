using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CodeQuestBackend.Models.Dtos;

public class CreatePostDto
{
    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es requerido")]
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "El resumen no puede exceder 500 caracteres")]
    public string? Summary { get; set; }

    public string? ImageUrl { get; set; }

    public int? CategoryId { get; set; }
    public int? SubcategoryId { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
}