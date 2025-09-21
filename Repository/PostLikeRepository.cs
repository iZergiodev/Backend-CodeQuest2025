using CodeQuestBackend.Data;
using CodeQuestBackend.Models;
using CodeQuestBackend.Repository.IRepository;
using CodeQuestBackend.Services;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Repository;

public class PostLikeRepository : IPostLikeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly TrendingService _trendingService;

    public PostLikeRepository(ApplicationDbContext context, TrendingService trendingService)
    {
        _context = context;
        _trendingService = trendingService;
    }

    public async Task<bool> LikePostAsync(int postId, int userId)
    {
        // Check if already liked
        var existingLike = await _context.Likes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

        if (existingLike != null)
            return false; // Already liked

        // Create new like
        var like = new Like
        {
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Likes.Add(like);

        // Update post likes count
        var post = await _context.Posts.FindAsync(postId);
        if (post != null)
        {
            post.LikesCount++;
        }

        await _context.SaveChangesAsync();

        // Record engagement for trending (after saving to avoid duplicate count increments)
        await _trendingService.RecordEngagementAsync(postId, userId, EngagementType.Like);
        
        return true;
    }

    public async Task<bool> UnlikePostAsync(int postId, int userId)
    {
        var existingLike = await _context.Likes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

        if (existingLike == null)
            return false; // Not liked

        _context.Likes.Remove(existingLike);

        // Update post likes count
        var post = await _context.Posts.FindAsync(postId);
        if (post != null)
        {
            post.LikesCount--;
        }

        await _context.SaveChangesAsync();

        // Record engagement removal for trending (after saving to avoid duplicate count decrements)
        await _trendingService.RemoveEngagementAsync(postId, userId, EngagementType.Like);
        
        return true;
    }

    public async Task<bool> IsLikedByUserAsync(int postId, int userId)
    {
        return await _context.Likes
            .AnyAsync(l => l.PostId == postId && l.UserId == userId);
    }

    public async Task<int> GetLikesCountAsync(int postId)
    {
        return await _context.Likes
            .CountAsync(l => l.PostId == postId);
    }
}
