using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeQuestBackend.Models;

public class CommentLike
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    // Foreign Keys
    [Required]
    public int CommentId { get; set; }

    [Required]
    public int UserId { get; set; }

    // Navigation Properties
    [ForeignKey("CommentId")]
    public virtual Comment Comment { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
