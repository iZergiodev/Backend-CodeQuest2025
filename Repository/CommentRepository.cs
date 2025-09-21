using CodeQuestBackend.Data;
using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CommentDto>> GetByPostIdAsync(int postId)
        {
            return await GetByPostIdAsync(postId, null);
        }

        public async Task<IEnumerable<CommentDto>> GetByPostIdAsync(int postId, int? currentUserId)
        {
            return await GetByPostIdAsync(postId, currentUserId, "newest");
        }

        public async Task<IEnumerable<CommentDto>> GetByPostIdAsync(int postId, int? currentUserId, string sortBy)
        {
            // Load all comments for the post with their authors
            var allCommentsQuery = _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.Author)
                .Include(c => c.Post)
                .Include(c => c.Likes);

            // Apply sorting
            var allComments = sortBy.ToLower() switch
            {
                "top" => await allCommentsQuery.OrderByDescending(c => c.LikesCount).ThenByDescending(c => c.CreatedAt).ToListAsync(),
                "controversial" => await allCommentsQuery.ToListAsync(), // We'll sort by replies count after loading
                "oldest" => await allCommentsQuery.OrderBy(c => c.CreatedAt).ToListAsync(),
                _ => await allCommentsQuery.OrderByDescending(c => c.CreatedAt).ToListAsync() // default to newest
            };

            // Convert to DTOs with recursive replies
            var commentDtos = allComments.Select(c => MapCommentToDto(c, allComments, currentUserId)).ToList();

            // Apply controversial sorting if needed (after we have the replies count)
            if (sortBy.ToLower() == "controversial")
            {
                commentDtos = commentDtos.OrderByDescending(c => c.RepliesCount).ThenByDescending(c => c.CreatedAt).ToList();
            }

            // Return only top-level comments (ParentId is null)
            return commentDtos.Where(c => c.ParentId == null).ToList();
        }

        private CommentDto MapCommentToDto(Comment comment, List<Comment> allComments, int? currentUserId = null)
        {
            var replies = allComments
                .Where(c => c.ParentId == comment.Id)
                .OrderBy(c => c.CreatedAt)
                .Select(c => MapCommentToDto(c, allComments, currentUserId))
                .ToList();

            // Ensure we have the author information
            var authorName = comment.Author?.Name ?? comment.Author?.Username ?? "Usuario";
            var authorAvatar = comment.Author?.Avatar;

            // Calculate total replies count (including nested replies)
            var totalRepliesCount = allComments.Count(c => c.ParentId == comment.Id);

            // Check if current user has liked this comment
            var isLikedByUser = currentUserId.HasValue && 
                               comment.Likes.Any(l => l.UserId == currentUserId.Value);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                PostId = comment.PostId,
                PostTitle = comment.Post?.Title ?? "Unknown Post",
                AuthorId = comment.AuthorId,
                AuthorName = authorName,
                AuthorAvatar = authorAvatar,
                ParentId = comment.ParentId,
                Replies = replies,
                LikesCount = comment.LikesCount,
                RepliesCount = totalRepliesCount,
                IsLikedByUser = isLikedByUser
            };
        }

        public async Task<IEnumerable<CommentDto>> GetByAuthorIdAsync(int authorId)
        {
            return await _context.Comments
                .Where(c => c.AuthorId == authorId)
                .Include(c => c.Author)
                .Include(c => c.Post)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    PostId = c.PostId,
                    PostTitle = c.Post.Title,
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.Name ?? c.Author.Username ?? "Unknown",
                    AuthorAvatar = c.Author.Avatar
                })
                .ToListAsync();
        }

        public async Task<Comment> CreateAsync(CreateCommentDto createCommentDto, int authorId)
        {
            var comment = new Comment
            {
                Content = createCommentDto.Content,
                PostId = createCommentDto.PostId,
                AuthorId = authorId,
                ParentId = createCommentDto.ParentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Update post comments count (count ALL comments, including nested ones)
            var post = await _context.Posts.FindAsync(createCommentDto.PostId);
            if (post != null)
            {
                post.CommentsCount++;
                await _context.SaveChangesAsync();
            }

            return comment;
        }

        public async Task<Comment?> UpdateAsync(int id, UpdateCommentDto updateCommentDto, int authorId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && c.AuthorId == authorId);

            if (comment == null)
                return null;

            comment.Content = updateCommentDto.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteAsync(int id, int authorId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && c.AuthorId == authorId);

            if (comment == null)
                return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            // Update post comments count
            var post = await _context.Posts.FindAsync(comment.PostId);
            if (post != null)
            {
                post.CommentsCount--;
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<int> GetCountByAuthorIdAsync(int authorId)
        {
            return await _context.Comments
                .CountAsync(c => c.AuthorId == authorId);
        }
    }
}
