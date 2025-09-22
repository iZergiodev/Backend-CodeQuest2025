using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using CodeQuestBackend.Data;

namespace CodeQuestBackend.Repository;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _context;

    public PostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Post>> GetAllAsync()
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ICollection<Post>> GetByAuthorIdAsync(int authorId)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<ICollection<Post>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Post> CreateAsync(CreatePostDto createPostDto, int authorId)
    {
        var post = new Post
        {
            Title = createPostDto.Title,
            Content = createPostDto.Content,
            Summary = createPostDto.Summary,
            ImageUrl = createPostDto.ImageUrl,
            Tags = createPostDto.Tags,
            AuthorId = authorId,
            CategoryId = createPostDto.CategoryId,
            SubcategoryId = createPostDto.SubcategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LikesCount = 0,
            CommentsCount = 0,
            VisitsCount = 0
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(post.Id) ?? post;
    }

    public async Task<Post?> UpdateAsync(int id, CreatePostDto updatePostDto)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return null;

        post.Title = updatePostDto.Title;
        post.Content = updatePostDto.Content;
        post.Summary = updatePostDto.Summary;
        post.ImageUrl = updatePostDto.ImageUrl;
        post.Tags = updatePostDto.Tags;
        post.CategoryId = updatePostDto.CategoryId;
        post.SubcategoryId = updatePostDto.SubcategoryId;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(post.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return false;

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Posts.AnyAsync(p => p.Id == id);
    }

    public async Task UpdateLikesCountAsync(int postId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post != null)
        {
            post.LikesCount = await _context.Likes.CountAsync(l => l.PostId == postId);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateCommentsCountAsync(int postId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post != null)
        {
            post.CommentsCount = await _context.Comments.CountAsync(c => c.PostId == postId);
            await _context.SaveChangesAsync();
        }
    }

    public async Task IncrementVisitsCountAsync(int postId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post != null)
        {
            post.VisitsCount++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetPostCountByCategoryAsync(int categoryId)
    {
        return await _context.Posts.CountAsync(p => p.CategoryId == categoryId);
    }

    public async Task<PaginatedResultDto<Post>> GetAllPaginatedAsync(int page, int pageSize)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .OrderByDescending(p => p.CreatedAt);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResultDto<Post>
        {
            Data = posts,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }

    public async Task<PaginatedResultDto<Post>> GetByAuthorIdPaginatedAsync(int authorId, int page, int pageSize)
    {
        var query = _context.Posts
            .Where(p => p.AuthorId == authorId)
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .OrderByDescending(p => p.CreatedAt);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResultDto<Post>
        {
            Data = posts,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }

    public async Task<PaginatedResultDto<Post>> GetByCategoryIdPaginatedAsync(int categoryId, int page, int pageSize)
    {
        var query = _context.Posts
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .OrderByDescending(p => p.CreatedAt);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResultDto<Post>
        {
            Data = posts,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }

    public async Task<PaginatedResultDto<Post>> GetBySubcategoryIdPaginatedAsync(int subcategoryId, int page, int pageSize)
    {
        var query = _context.Posts
            .Where(p => p.SubcategoryId == subcategoryId)
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .OrderByDescending(p => p.CreatedAt);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResultDto<Post>
        {
            Data = posts,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }

    public async Task<PaginatedResultDto<Post>> GetByFollowedSubcategoriesAsync(List<int> subcategoryIds, int page, int pageSize, string sortBy = "recent")
    {
        if (subcategoryIds == null || !subcategoryIds.Any())
        {
            return new PaginatedResultDto<Post>
            {
                Data = new List<Post>(),
                Page = page,
                PageSize = pageSize,
                TotalItems = 0,
                TotalPages = 0,
                HasNextPage = false,
                HasPreviousPage = false
            };
        }

        var baseQuery = _context.Posts
            .Where(p => subcategoryIds.Contains(p.SubcategoryId ?? 0))
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory);

        // Apply sorting
        var query = sortBy.ToLower() switch
        {
            "popular" => baseQuery.OrderByDescending(p => p.LikesCount + p.CommentsCount + p.VisitsCount),
            "trending" => baseQuery.OrderByDescending(p => p.TrendingScore),
            "oldest" => baseQuery.OrderBy(p => p.CreatedAt),
            "recent" or _ => baseQuery.OrderByDescending(p => p.CreatedAt)
        };

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResultDto<Post>
        {
            Data = posts,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }

    public async Task<PaginatedResultDto<Post>> SearchPostsAsync(string query, int page, int pageSize, string sortBy = "recent")
    {
        // Build base query with includes
        var baseQuery = _context.Posts
            .Where(p => p.Title.Contains(query) || p.Content.Contains(query) || (p.Summary != null && p.Summary.Contains(query)))
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory);

        // Apply sorting
        var queryResult = sortBy.ToLower() switch
        {
            "popular" => baseQuery.OrderByDescending(p => p.LikesCount + p.CommentsCount + p.VisitsCount),
            "trending" => baseQuery.OrderByDescending(p => p.TrendingScore),
            "oldest" => baseQuery.OrderBy(p => p.CreatedAt),
            "recent" or _ => baseQuery.OrderByDescending(p => p.CreatedAt)
        };

        var totalItems = await queryResult.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var posts = await queryResult
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResultDto<Post>
        {
            Data = posts,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }

    public async Task<ICollection<Post>> GetRelatedPostsAsync(int postId, int limit = 5)
    {
        // Get the current post to find its tags and subcategory
        var currentPost = await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (currentPost == null)
            return new List<Post>();

        var relatedPosts = new List<Post>();

        // Get all posts with includes for efficient querying
        var allPosts = await _context.Posts
            .Where(p => p.Id != postId)
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .ToListAsync();

        // First priority: Posts with matching tags
        if (currentPost.Tags != null && currentPost.Tags.Length > 0)
        {
            var tagMatches = allPosts
                .Where(p => p.Tags != null && p.Tags.Any(tag => currentPost.Tags.Contains(tag)))
                .OrderByDescending(p => p.CreatedAt)
                .Take(limit)
                .ToList();

            relatedPosts.AddRange(tagMatches);
        }

        // Second priority: Posts from the same subcategory (only if we don't have enough from tags)
        if (relatedPosts.Count < limit && currentPost.SubcategoryId.HasValue)
        {
            var subcategoryMatches = allPosts
                .Where(p => p.SubcategoryId == currentPost.SubcategoryId && 
                           !relatedPosts.Any(rp => rp.Id == p.Id))
                .OrderByDescending(p => p.CreatedAt)
                .Take(limit - relatedPosts.Count)
                .ToList();

            relatedPosts.AddRange(subcategoryMatches);
        }

        // Third priority: Posts from the same category (only if we don't have enough from tags/subcategory)
        if (relatedPosts.Count < limit && currentPost.CategoryId.HasValue)
        {
            var categoryMatches = allPosts
                .Where(p => p.CategoryId == currentPost.CategoryId && 
                           !relatedPosts.Any(rp => rp.Id == p.Id))
                .OrderByDescending(p => p.CreatedAt)
                .Take(limit - relatedPosts.Count)
                .ToList();

            relatedPosts.AddRange(categoryMatches);
        }

        // Only return posts that are truly related (by tags, subcategory, or category)
        // Don't fall back to random recent posts - if there are no related posts, return empty list

        return relatedPosts.Take(limit).ToList();
    }
}