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
            var allComments = await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.Author)
                .Include(c => c.Post)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Author)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            // Convert to DTOs with recursive replies
            var commentDtos = allComments.Select(c => MapCommentToDto(c, allComments)).ToList();

            // Return only top-level comments (ParentId is null)
            return commentDtos.Where(c => c.ParentId == null).ToList();
        }

        private CommentDto MapCommentToDto(Comment comment, List<Comment> allComments)
        {
            var replies = allComments
                .Where(c => c.ParentId == comment.Id)
                .OrderBy(c => c.CreatedAt)
                .Select(c => MapCommentToDto(c, allComments))
                .ToList();

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                PostId = comment.PostId,
                PostTitle = comment.Post.Title,
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author.Name ?? comment.Author.Username ?? "Unknown",
                AuthorAvatar = comment.Author.Avatar,
                ParentId = comment.ParentId,
                Replies = replies
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

            // Update post comments count (only for top-level comments)
            if (createCommentDto.ParentId == null)
            {
                var post = await _context.Posts.FindAsync(createCommentDto.PostId);
                if (post != null)
                {
                    post.CommentsCount++;
                    await _context.SaveChangesAsync();
                }
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
