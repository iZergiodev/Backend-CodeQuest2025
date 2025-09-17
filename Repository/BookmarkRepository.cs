using CodeQuestBackend.Data;
using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Repository
{
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly ApplicationDbContext _context;

        public BookmarkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Bookmark?> GetBookmarkAsync(int userId, int postId)
        {
            return await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.PostId == postId);
        }

        public async Task<Bookmark> CreateBookmarkAsync(int userId, int postId)
        {
            var bookmark = new Bookmark
            {
                UserId = userId,
                PostId = postId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookmarks.Add(bookmark);
            await _context.SaveChangesAsync();
            return bookmark;
        }

        public async Task<bool> DeleteBookmarkAsync(int userId, int postId)
        {
            var bookmark = await GetBookmarkAsync(userId, postId);
            if (bookmark == null)
                return false;

            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsBookmarkedAsync(int userId, int postId)
        {
            return await _context.Bookmarks
                .AnyAsync(b => b.UserId == userId && b.PostId == postId);
        }

        public async Task<int> GetBookmarkCountAsync(int postId)
        {
            return await _context.Bookmarks
                .CountAsync(b => b.PostId == postId);
        }

        public async Task<List<BookmarkDto>> GetUserBookmarksAsync(int userId, int page = 1, int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;

            return await _context.Bookmarks
                .Where(b => b.UserId == userId)
                .Include(b => b.Post)
                    .ThenInclude(p => p.Category)
                .Include(b => b.Post)
                    .ThenInclude(p => p.Subcategory)
                .Include(b => b.Post)
                    .ThenInclude(p => p.Author)
                .OrderByDescending(b => b.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Select(b => new BookmarkDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    PostId = b.PostId,
                    CreatedAt = b.CreatedAt,
                    Post = new PostDto
                    {
                        Id = b.Post.Id,
                        Title = b.Post.Title,
                        Summary = b.Post.Summary,
                        ImageUrl = b.Post.ImageUrl,
                        Tags = b.Post.Tags,
                        CreatedAt = b.Post.CreatedAt,
                        UpdatedAt = b.Post.UpdatedAt,
                        AuthorId = b.Post.AuthorId,
                        AuthorName = b.Post.Author.Name ?? b.Post.Author.Username ?? "Unknown",
                        CategoryId = b.Post.CategoryId,
                        CategoryName = b.Post.Category != null ? b.Post.Category.Name : null,
                        CategoryColor = b.Post.Category != null ? b.Post.Category.Color : null,
                        Category = b.Post.Category != null ? new CategoryDto
                        {
                            Id = b.Post.Category.Id,
                            Name = b.Post.Category.Name,
                            Description = b.Post.Category.Description,
                            Color = b.Post.Category.Color,
                            CreatedAt = b.Post.Category.CreatedAt,
                            UpdatedAt = b.Post.Category.UpdatedAt
                        } : null,
                        SubcategoryId = b.Post.SubcategoryId,
                        SubcategoryName = b.Post.Subcategory != null ? b.Post.Subcategory.Name : null,
                        SubcategoryColor = b.Post.Subcategory != null ? b.Post.Subcategory.Color : null,
                        Subcategory = b.Post.Subcategory != null ? new SubcategoryDto
                        {
                            Id = b.Post.Subcategory.Id,
                            Name = b.Post.Subcategory.Name,
                            Description = b.Post.Subcategory.Description,
                            Color = b.Post.Subcategory.Color,
                            CategoryId = b.Post.Subcategory.CategoryId,
                            CategoryName = b.Post.Subcategory.Category != null ? b.Post.Subcategory.Category.Name : "",
                            CreatedAt = b.Post.Subcategory.CreatedAt,
                            UpdatedAt = b.Post.Subcategory.UpdatedAt
                        } : null,
                        LikesCount = b.Post.LikesCount,
                        CommentsCount = b.Post.CommentsCount,
                        VisitsCount = b.Post.VisitsCount
                    }
                })
                .ToListAsync();
        }

        public async Task<int> GetUserBookmarkCountAsync(int userId)
        {
            return await _context.Bookmarks
                .CountAsync(b => b.UserId == userId);
        }

        public async Task<List<BookmarkDto>> GetBookmarksByPostAsync(int postId, int page = 1, int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;

            return await _context.Bookmarks
                .Where(b => b.PostId == postId)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Select(b => new BookmarkDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    PostId = b.PostId,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
        }
    }
}
