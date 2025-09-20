using CodeQuestBackend.Data;
using CodeQuestBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Services;

public class PopularityService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PopularityService> _logger;

    // Popularity algorithm weights
    private const double VIEW_WEIGHT = 1.0;
    private const double LIKE_WEIGHT = 2.0;
    private const double COMMENT_WEIGHT = 3.0;
    private const double BOOKMARK_WEIGHT = 1.5;

    public PopularityService(ApplicationDbContext context, ILogger<PopularityService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Calculates popularity scores for all posts based on cumulative engagement
    /// </summary>
    public async Task CalculatePopularityScoresAsync()
    {
        try
        {
            _logger.LogInformation("Starting popularity score calculation");

            var posts = await _context.Posts.ToListAsync();

            foreach (var post in posts)
            {
                var popularityScore = CalculatePostPopularityScore(post);
                post.PopularityScore = popularityScore;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Updated popularity scores for {posts.Count} posts");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating popularity scores");
            throw;
        }
    }

    /// <summary>
    /// Calculates popularity score for a specific post
    /// </summary>
    private double CalculatePostPopularityScore(Post post)
    {
        // Simple weighted sum algorithm
        var score = (post.VisitsCount * VIEW_WEIGHT) +
                   (post.LikesCount * LIKE_WEIGHT) +
                   (post.CommentsCount * COMMENT_WEIGHT) +
                   (post.Bookmarks.Count * BOOKMARK_WEIGHT);

        // Apply age decay (older posts get slightly lower scores)
        var daysSinceCreation = (DateTime.UtcNow - post.CreatedAt).TotalDays;
        var ageDecay = Math.Pow(0.99, daysSinceCreation); // 1% decay per day

        return score * ageDecay;
    }

    /// <summary>
    /// Gets popular posts ordered by popularity score
    /// </summary>
    public async Task<List<Post>> GetPopularPostsAsync(int limit = 20, int? categoryId = null, int? subcategoryId = null)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .Include(p => p.Bookmarks)
            .Where(p => p.PopularityScore > 0);

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        if (subcategoryId.HasValue)
        {
            query = query.Where(p => p.SubcategoryId == subcategoryId);
        }

        return await query
            .OrderByDescending(p => p.PopularityScore)
            .ThenByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    /// <summary>
    /// Updates popularity score for a specific post
    /// </summary>
    public async Task UpdatePostPopularityScoreAsync(int postId)
    {
        try
        {
            var post = await _context.Posts
                .Include(p => p.Bookmarks)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post != null)
            {
                var popularityScore = CalculatePostPopularityScore(post);
                post.PopularityScore = popularityScore;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating popularity score for post {postId}");
            throw;
        }
    }

    /// <summary>
    /// Gets posts by popularity within a time range
    /// </summary>
    public async Task<List<Post>> GetPopularPostsInTimeRangeAsync(DateTime startDate, DateTime endDate, int limit = 20)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .Include(p => p.Bookmarks)
            .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
            .OrderByDescending(p => p.PopularityScore)
            .ThenByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    /// <summary>
    /// Gets the most popular posts of all time
    /// </summary>
    public async Task<List<Post>> GetMostPopularPostsAsync(int limit = 10)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .Include(p => p.Bookmarks)
            .OrderByDescending(p => p.PopularityScore)
            .Take(limit)
            .ToListAsync();
    }

    /// <summary>
    /// Gets popular posts by category
    /// </summary>
    public async Task<Dictionary<string, List<Post>>> GetPopularPostsByCategoryAsync(int limitPerCategory = 5)
    {
        var categories = await _context.Categories.ToListAsync();
        var result = new Dictionary<string, List<Post>>();

        foreach (var category in categories)
        {
            var popularPosts = await GetPopularPostsAsync(limitPerCategory, category.Id);
            result[category.Name] = popularPosts;
        }

        return result;
    }
}
