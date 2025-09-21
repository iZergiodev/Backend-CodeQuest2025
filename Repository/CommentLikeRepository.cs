using CodeQuestBackend.Data;
using CodeQuestBackend.Models;
using CodeQuestBackend.Repository.IRepository;
using CodeQuestBackend.Services;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Repository;

public class CommentLikeRepository : ICommentLikeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly TrendingService _trendingService;

    public CommentLikeRepository(ApplicationDbContext context, TrendingService trendingService)
    {
        _context = context;
        _trendingService = trendingService;
    }

    public async Task<bool> LikeCommentAsync(int commentId, int userId)
    {
        // Check if already liked
        var existingLike = await _context.CommentLikes
            .FirstOrDefaultAsync(cl => cl.CommentId == commentId && cl.UserId == userId);

        if (existingLike != null)
            return false; // Already liked

        // Create new like
        var commentLike = new CommentLike
        {
            CommentId = commentId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.CommentLikes.Add(commentLike);

        // Update comment likes count
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment != null)
        {
            comment.LikesCount++;
        }

        await _context.SaveChangesAsync();

        // Record engagement for trending (affects the post's trending score)
        if (comment != null)
        {
            await _trendingService.RecordEngagementAsync(comment.PostId, userId, EngagementType.Like);
        }
        
        return true;
    }

    public async Task<bool> UnlikeCommentAsync(int commentId, int userId)
    {
        var existingLike = await _context.CommentLikes
            .FirstOrDefaultAsync(cl => cl.CommentId == commentId && cl.UserId == userId);

        if (existingLike == null)
            return false; // Not liked

        _context.CommentLikes.Remove(existingLike);

        // Update comment likes count
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment != null)
        {
            comment.LikesCount--;
        }

        await _context.SaveChangesAsync();

        // Record engagement removal for trending (affects the post's trending score)
        if (comment != null)
        {
            await _trendingService.RemoveEngagementAsync(comment.PostId, userId, EngagementType.Like);
        }
        
        return true;
    }

    public async Task<bool> IsLikedByUserAsync(int commentId, int userId)
    {
        return await _context.CommentLikes
            .AnyAsync(cl => cl.CommentId == commentId && cl.UserId == userId);
    }

    public async Task<int> GetLikesCountAsync(int commentId)
    {
        return await _context.CommentLikes
            .CountAsync(cl => cl.CommentId == commentId);
    }
}
