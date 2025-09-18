using CodeQuestBackend.Data;
using CodeQuestBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Services;

public class TrendingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TrendingService> _logger;

    // Trending algorithm weights
    private const double VIEW_WEIGHT = 1.0;
    private const double LIKE_WEIGHT = 2.0;
    private const double COMMENT_WEIGHT = 3.0;
    private const double BOOKMARK_WEIGHT = 1.5;

    // Time decay parameters
    private const int TRENDING_WINDOW_HOURS = 24;
    private const double DECAY_FACTOR = 0.1; // Higher = more decay

    public TrendingService(ApplicationDbContext context, ILogger<TrendingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Calculates trending scores for all posts based on recent activity
    /// </summary>
    public async Task CalculateTrendingScoresAsync()
    {
        try
        {
            _logger.LogInformation("Starting trending score calculation");

            var cutoffTime = DateTime.UtcNow.AddHours(-TRENDING_WINDOW_HOURS);
            
            // Get all posts with recent activity
            var posts = await _context.Posts
                .Where(p => p.LastActivityAt >= cutoffTime)
                .ToListAsync();

            foreach (var post in posts)
            {
                var trendingScore = await CalculatePostTrendingScoreAsync(post.Id, cutoffTime);
                post.TrendingScore = trendingScore;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Updated trending scores for {posts.Count} posts");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating trending scores");
            throw;
        }
    }

    /// <summary>
    /// Calculates trending score for a specific post
    /// </summary>
    private async Task<double> CalculatePostTrendingScoreAsync(int postId, DateTime cutoffTime)
    {
        // Get recent engagement events for this post
        var recentEvents = await _context.EngagementEvents
            .Where(e => e.PostId == postId && e.CreatedAt >= cutoffTime)
            .ToListAsync();

        double totalScore = 0;
        var now = DateTime.UtcNow;

        foreach (var evt in recentEvents)
        {
            // Calculate time decay (more recent = higher score)
            var hoursAgo = (now - evt.CreatedAt).TotalHours;
            var timeDecay = Math.Exp(-DECAY_FACTOR * hoursAgo);

            // Calculate base score based on engagement type
            double baseScore = evt.Type switch
            {
                EngagementType.View => VIEW_WEIGHT,
                EngagementType.Like => LIKE_WEIGHT,
                EngagementType.Comment => COMMENT_WEIGHT,
                EngagementType.Bookmark => BOOKMARK_WEIGHT,
                _ => 0
            };

            totalScore += baseScore * timeDecay;
        }

        return totalScore;
    }

    /// <summary>
    /// Gets trending posts ordered by trending score
    /// </summary>
    public async Task<List<Post>> GetTrendingPostsAsync(int limit = 20, int? categoryId = null, int? subcategoryId = null)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .Where(p => p.TrendingScore > 0);

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        if (subcategoryId.HasValue)
        {
            query = query.Where(p => p.SubcategoryId == subcategoryId);
        }

        return await query
            .OrderByDescending(p => p.TrendingScore)
            .ThenByDescending(p => p.LastActivityAt)
            .Take(limit)
            .ToListAsync();
    }

    /// <summary>
    /// Records an engagement event and updates post metrics
    /// </summary>
    public async Task RecordEngagementAsync(int postId, int userId, EngagementType type)
    {
        try
        {
            // Create engagement event
            var engagementEvent = new EngagementEvent
            {
                PostId = postId,
                UserId = userId,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            _context.EngagementEvents.Add(engagementEvent);

            // Update post metrics
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                post.LastActivityAt = DateTime.UtcNow;

                // Update recent counts (last 24 hours)
                var cutoffTime = DateTime.UtcNow.AddHours(-24);
                var recentCount = await _context.EngagementEvents
                    .CountAsync(e => e.PostId == postId && e.Type == type && e.CreatedAt >= cutoffTime);

                switch (type)
                {
                    case EngagementType.View:
                        post.VisitsCount++;
                        post.RecentVisitsCount = recentCount;
                        break;
                    case EngagementType.Like:
                        post.LikesCount++;
                        post.RecentLikesCount = recentCount;
                        break;
                    case EngagementType.Comment:
                        post.CommentsCount++;
                        post.RecentCommentsCount = recentCount;
                        break;
                }

                // Recalculate trending score for this post
                var trendingScore = await CalculatePostTrendingScoreAsync(postId, DateTime.UtcNow.AddHours(-TRENDING_WINDOW_HOURS));
                post.TrendingScore = trendingScore;
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error recording engagement for post {postId}, user {userId}, type {type}");
            throw;
        }
    }

    /// <summary>
    /// Removes an engagement event (e.g., when user unlikes)
    /// </summary>
    public async Task RemoveEngagementAsync(int postId, int userId, EngagementType type)
    {
        try
        {
            var engagementEvent = await _context.EngagementEvents
                .FirstOrDefaultAsync(e => e.PostId == postId && e.UserId == userId && e.Type == type);

            if (engagementEvent != null)
            {
                _context.EngagementEvents.Remove(engagementEvent);

                // Update post metrics
                var post = await _context.Posts.FindAsync(postId);
                if (post != null)
                {
                    switch (type)
                    {
                        case EngagementType.View:
                            post.VisitsCount = Math.Max(0, post.VisitsCount - 1);
                            break;
                        case EngagementType.Like:
                            post.LikesCount = Math.Max(0, post.LikesCount - 1);
                            break;
                        case EngagementType.Comment:
                            post.CommentsCount = Math.Max(0, post.CommentsCount - 1);
                            break;
                    }

                    // Recalculate trending score
                    var trendingScore = await CalculatePostTrendingScoreAsync(postId, DateTime.UtcNow.AddHours(-TRENDING_WINDOW_HOURS));
                    post.TrendingScore = trendingScore;
                }

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing engagement for post {postId}, user {userId}, type {type}");
            throw;
        }
    }
}
