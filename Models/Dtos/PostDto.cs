namespace CodeQuestBackend.Models.Dtos;

public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int AuthorId { get; set; }
    public string? AuthorAvatar { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryColor { get; set; }
    public CategoryDto? Category { get; set; }
    public int? SubcategoryId { get; set; }
    public string? SubcategoryName { get; set; }
    public string? SubcategoryColor { get; set; }
    public SubcategoryDto? Subcategory { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int VisitsCount { get; set; }
    public bool IsLikedByUser { get; set; }
    
    // Engagement Metrics
    public double PopularityScore { get; set; }
    public double TrendingScore { get; set; }
    public DateTime LastActivityAt { get; set; }
    public int RecentLikesCount { get; set; }
    public int RecentCommentsCount { get; set; }
    public int RecentVisitsCount { get; set; }
}