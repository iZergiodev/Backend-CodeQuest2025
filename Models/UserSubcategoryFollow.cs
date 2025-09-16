using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeQuestBackend.Models;

public class UserSubcategoryFollow
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int SubcategoryId { get; set; }

    public DateTime FollowedAt { get; set; }

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("SubcategoryId")]
    public virtual Subcategory Subcategory { get; set; } = null!;
}

