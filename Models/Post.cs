using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeQuestBackend.Models;

public class Post
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Summary { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Foreign Keys
    [Required]
    public int AuthorId { get; set; }

    public int? CategoryId { get; set; }

    // Navigation Properties
    [ForeignKey("AuthorId")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    // Counters
    public int LikesCount { get; set; } = 0;
    public int CommentsCount { get; set; } = 0;
}