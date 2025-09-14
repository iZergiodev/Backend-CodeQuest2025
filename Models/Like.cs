using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeQuestBackend.Models;

public class Like
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    // Foreign Keys
    [Required]
    public int PostId { get; set; }

    [Required]
    public int UserId { get; set; }

    // Navigation Properties
    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}