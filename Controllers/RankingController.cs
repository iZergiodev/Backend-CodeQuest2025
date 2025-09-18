using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeQuestBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RankingController : ControllerBase
{
    private readonly TrendingService _trendingService;
    private readonly PopularityService _popularityService;
    private readonly ILogger<RankingController> _logger;

    public RankingController(
        TrendingService trendingService, 
        PopularityService popularityService,
        ILogger<RankingController> logger)
    {
        _trendingService = trendingService;
        _popularityService = popularityService;
        _logger = logger;
    }

    /// <summary>
    /// Gets trending posts
    /// </summary>
    [HttpGet("trending")]
    public async Task<IActionResult> GetTrendingPosts(
        [FromQuery] int limit = 20,
        [FromQuery] int? categoryId = null,
        [FromQuery] int? subcategoryId = null)
    {
        try
        {
            var posts = await _trendingService.GetTrendingPostsAsync(limit, categoryId, subcategoryId);
            var postDtos = posts.Select(MapToPostDto).ToList();
            
            return Ok(new
            {
                posts = postDtos,
                count = postDtos.Count,
                type = "trending"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending posts");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Error retrieving trending posts");
        }
    }

    /// <summary>
    /// Gets popular posts
    /// </summary>
    [HttpGet("popular")]
    public async Task<IActionResult> GetPopularPosts(
        [FromQuery] int limit = 20,
        [FromQuery] int? categoryId = null,
        [FromQuery] int? subcategoryId = null)
    {
        try
        {
            var posts = await _popularityService.GetPopularPostsAsync(limit, categoryId, subcategoryId);
            var postDtos = posts.Select(MapToPostDto).ToList();
            
            return Ok(new
            {
                posts = postDtos,
                count = postDtos.Count,
                type = "popular"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular posts");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Error retrieving popular posts");
        }
    }

    /// <summary>
    /// Gets most popular posts of all time
    /// </summary>
    [HttpGet("most-popular")]
    public async Task<IActionResult> GetMostPopularPosts([FromQuery] int limit = 10)
    {
        try
        {
            var posts = await _popularityService.GetMostPopularPostsAsync(limit);
            var postDtos = posts.Select(MapToPostDto).ToList();
            
            return Ok(new
            {
                posts = postDtos,
                count = postDtos.Count,
                type = "most-popular"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting most popular posts");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Error retrieving most popular posts");
        }
    }

    /// <summary>
    /// Gets popular posts by category
    /// </summary>
    [HttpGet("popular-by-category")]
    public async Task<IActionResult> GetPopularPostsByCategory([FromQuery] int limitPerCategory = 5)
    {
        try
        {
            var postsByCategory = await _popularityService.GetPopularPostsByCategoryAsync(limitPerCategory);
            var result = postsByCategory.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(MapToPostDto).ToList()
            );
            
            return Ok(new
            {
                postsByCategory = result,
                type = "popular-by-category"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular posts by category");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Error retrieving popular posts by category");
        }
    }

    /// <summary>
    /// Records a view engagement
    /// </summary>
    [HttpPost("record-view")]
    public async Task<IActionResult> RecordView([FromBody] RecordEngagementRequest request)
    {
        try
        {
            await _trendingService.RecordEngagementAsync(request.PostId, request.UserId, EngagementType.View);
            return Ok(new { message = "View recorded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording view");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Error recording view");
        }
    }

    /// <summary>
    /// Records a like engagement
    /// </summary>
    [HttpPost("record-like")]
    public async Task<IActionResult> RecordLike([FromBody] RecordEngagementRequest request)
    {
        try
        {
            await _trendingService.RecordEngagementAsync(request.PostId, request.UserId, EngagementType.Like);
            return Ok(new { message = "Like recorded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording like");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Error recording like");
        }
    }

    /// <summary>
    /// Records a comment engagement
    /// </summary>
    [HttpPost("record-comment")]
    public async Task<IActionResult> RecordComment([FromBody] RecordEngagementRequest request)
    {
        try
        {
            await _trendingService.RecordEngagementAsync(request.PostId, request.UserId, EngagementType.Comment);
            return Ok(new { message = "Comment recorded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording comment");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Error recording comment");
        }
    }

    /// <summary>
    /// Records a bookmark engagement
    /// </summary>
    [HttpPost("record-bookmark")]
    public async Task<IActionResult> RecordBookmark([FromBody] RecordEngagementRequest request)
    {
        try
        {
            await _trendingService.RecordEngagementAsync(request.PostId, request.UserId, EngagementType.Bookmark);
            return Ok(new { message = "Bookmark recorded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording bookmark");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Error recording bookmark");
        }
    }

    /// <summary>
    /// Removes an engagement (e.g., when user unlikes)
    /// </summary>
    [HttpDelete("remove-engagement")]
    public async Task<IActionResult> RemoveEngagement([FromBody] RemoveEngagementRequest request)
    {
        try
        {
            await _trendingService.RemoveEngagementAsync(request.PostId, request.UserId, request.Type);
            return Ok(new { message = "Engagement removed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing engagement");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Error removing engagement");
        }
    }

    /// <summary>
    /// Triggers recalculation of trending scores
    /// </summary>
    [HttpPost("recalculate-trending")]
    public async Task<IActionResult> RecalculateTrending()
    {
        try
        {
            await _trendingService.CalculateTrendingScoresAsync();
            return Ok(new { message = "Trending scores recalculated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recalculating trending scores");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Error recalculating trending scores");
        }
    }

    /// <summary>
    /// Triggers recalculation of popularity scores
    /// </summary>
    [HttpPost("recalculate-popularity")]
    public async Task<IActionResult> RecalculatePopularity()
    {
        try
        {
            await _popularityService.CalculatePopularityScoresAsync();
            return Ok(new { message = "Popularity scores recalculated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recalculating popularity scores");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Error recalculating popularity scores");
        }
    }

    private static PostDto MapToPostDto(Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Summary = post.Summary,
            ImageUrl = post.ImageUrl,
            Tags = post.Tags,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            AuthorId = post.AuthorId,
            AuthorName = post.Author?.Name ?? post.Author?.Username ?? "Unknown",
            AuthorAvatar = post.Author?.Avatar,
            CategoryId = post.CategoryId,
            CategoryName = post.Category?.Name,
            CategoryColor = post.Category?.Color,
            SubcategoryId = post.SubcategoryId,
            SubcategoryName = post.Subcategory?.Name,
            SubcategoryColor = post.Subcategory?.Color,
            LikesCount = post.LikesCount,
            CommentsCount = post.CommentsCount,
            VisitsCount = post.VisitsCount,
            PopularityScore = post.PopularityScore,
            TrendingScore = post.TrendingScore,
            LastActivityAt = post.LastActivityAt,
            RecentLikesCount = post.RecentLikesCount,
            RecentCommentsCount = post.RecentCommentsCount,
            RecentVisitsCount = post.RecentVisitsCount
        };
    }
}

public class RecordEngagementRequest
{
    public int PostId { get; set; }
    public int UserId { get; set; }
}

public class RemoveEngagementRequest
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public EngagementType Type { get; set; }
}
