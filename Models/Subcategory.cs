using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeQuestBackend.Models;

public class Subcategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Color { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Foreign Key
    [Required]
    public int CategoryId { get; set; }

    // Navigation Property
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;

    // Navigation property for posts
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}