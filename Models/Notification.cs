using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeQuestBackend.Models;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty; // "like", "comment", "follow", "post_in_followed_subcategory", "stardust_milestone"

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public int? RelatedPostId { get; set; }
    public int? RelatedCommentId { get; set; }
    public int? RelatedUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("RelatedPostId")]
    public virtual Post? RelatedPost { get; set; }

    [ForeignKey("RelatedCommentId")]
    public virtual Comment? RelatedComment { get; set; }

    [ForeignKey("RelatedUserId")]
    public virtual User? RelatedUser { get; set; }
}