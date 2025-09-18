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

    public string[] Tags { get; set; } = Array.Empty<string>();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Foreign Keys
    [Required]
    public int AuthorId { get; set; }

    public int? CategoryId { get; set; }

    public int? SubcategoryId { get; set; }

    // Navigation Properties
    [ForeignKey("AuthorId")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }

    [ForeignKey("SubcategoryId")]
    public virtual Subcategory? Subcategory { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    // Counters
    public int LikesCount { get; set; } = 0;
    public int CommentsCount { get; set; } = 0;
    public int VisitsCount { get; set; } = 0;
    
    // Engagement Metrics for Popularity and Trending
    public double PopularityScore { get; set; } = 0;
    public double TrendingScore { get; set; } = 0;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    public int RecentLikesCount { get; set; } = 0; // Last 24 hours
    public int RecentCommentsCount { get; set; } = 0; // Last 24 hours
    public int RecentVisitsCount { get; set; } = 0; // Last 24 hours
}