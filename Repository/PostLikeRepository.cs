using CodeQuestBackend.Data;
using CodeQuestBackend.Models;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Repository;

public class PostLikeRepository : IPostLikeRepository
{
    private readonly ApplicationDbContext _context;

    public PostLikeRepository(ApplicationDbContext context)
    {
        _context = context;
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
