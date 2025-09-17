using CodeQuestBackend.Data;
using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Repository;

public class UserFollowRepository : IUserFollowRepository
{
    private readonly ApplicationDbContext _context;

    public UserFollowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> FollowAllSubcategoriesAsync(int userId, int categoryId)
    {
        // Check if category exists
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == categoryId);
        if (!categoryExists)
            return false;

        // Get all subcategories for this category
        var subcategoryIds = await _context.Subcategories
            .Where(s => s.CategoryId == categoryId)
            .Select(s => s.Id)
            .ToListAsync();

        if (!subcategoryIds.Any())
            return false;

        // Get existing follows to avoid duplicates
        var existingFollows = await _context.UserSubcategoryFollows
            .Where(usf => usf.UserId == userId && subcategoryIds.Contains(usf.SubcategoryId))
            .Select(usf => usf.SubcategoryId)
            .ToListAsync();

        // Create follows for subcategories not already followed
        var newFollows = subcategoryIds
            .Where(id => !existingFollows.Contains(id))
            .Select(id => new UserSubcategoryFollow
            {
                UserId = userId,
                SubcategoryId = id,
                FollowedAt = DateTime.UtcNow
            })
            .ToList();

        if (newFollows.Any())
        {
            _context.UserSubcategoryFollows.AddRange(newFollows);
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> FollowSubcategoryAsync(int userId, int subcategoryId)
    {
        // Check if already following
        var existingFollow = await _context.UserSubcategoryFollows
            .FirstOrDefaultAsync(usf => usf.UserId == userId && usf.SubcategoryId == subcategoryId);

        if (existingFollow != null)
            return false; // Already following

        // Check if subcategory exists
        var subcategoryExists = await _context.Subcategories.AnyAsync(s => s.Id == subcategoryId);
        if (!subcategoryExists)
            return false;

        var follow = new UserSubcategoryFollow
        {
            UserId = userId,
            SubcategoryId = subcategoryId,
            FollowedAt = DateTime.UtcNow
        };

        _context.UserSubcategoryFollows.Add(follow);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnfollowSubcategoryAsync(int userId, int subcategoryId)
    {
        var follow = await _context.UserSubcategoryFollows
            .FirstOrDefaultAsync(usf => usf.UserId == userId && usf.SubcategoryId == subcategoryId);

        if (follow == null)
            return false; // Not following

        _context.UserSubcategoryFollows.Remove(follow);
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<bool> IsFollowingSubcategoryAsync(int userId, int subcategoryId)
    {
        return await _context.UserSubcategoryFollows
            .AnyAsync(usf => usf.UserId == userId && usf.SubcategoryId == subcategoryId);
    }

    public async Task<UserFollowsDto> GetUserFollowsAsync(int userId)
    {
        var followedSubcategories = await _context.UserSubcategoryFollows
            .Where(usf => usf.UserId == userId)
            .Include(usf => usf.Subcategory)
            .ThenInclude(s => s.Category)
            .Select(usf => new SubcategoryDto
            {
                Id = usf.Subcategory.Id,
                Name = usf.Subcategory.Name,
                Description = usf.Subcategory.Description,
                Color = usf.Subcategory.Color,
                CreatedAt = usf.Subcategory.CreatedAt,
                UpdatedAt = usf.Subcategory.UpdatedAt,
                CategoryId = usf.Subcategory.CategoryId,
                CategoryName = usf.Subcategory.Category.Name
            })
            .ToListAsync();

        return new UserFollowsDto
        {
            FollowedSubcategories = followedSubcategories
        };
    }

    public async Task<List<SubcategoryWithFollowerCountDto>> GetSubcategoriesWithFollowerCountAsync(int? userId = null)
    {
        var query = _context.Subcategories
            .Include(s => s.Category)
            .Where(s => s.Category != null)
            .GroupJoin(
                _context.UserSubcategoryFollows,
                s => s.Id,
                usf => usf.SubcategoryId,
                (s, follows) => new { Subcategory = s, Follows = follows }
            )
            .Select(x => new SubcategoryWithFollowerCountDto
            {
                Id = x.Subcategory.Id,
                Name = x.Subcategory.Name,
                Description = x.Subcategory.Description,
                Color = x.Subcategory.Color == null ? "#6B7280" : x.Subcategory.Color,
                CreatedAt = x.Subcategory.CreatedAt,
                UpdatedAt = x.Subcategory.UpdatedAt,
                CategoryId = x.Subcategory.CategoryId,
                CategoryName = x.Subcategory.Category.Name,
                FollowerCount = x.Follows.Count(),
                IsFollowing = userId.HasValue && x.Follows.Any(f => f.UserId == userId.Value)
            });

        return await query.ToListAsync();
    }

    public async Task<SubcategoryWithFollowerCountDto> GetSubcategoryWithFollowerCountAsync(int subcategoryId, int? userId = null)
    {
        var result = await _context.Subcategories
            .Where(s => s.Id == subcategoryId)
            .Include(s => s.Category)
            .Where(s => s.Category != null)
            .GroupJoin(
                _context.UserSubcategoryFollows,
                s => s.Id,
                usf => usf.SubcategoryId,
                (s, follows) => new { Subcategory = s, Follows = follows }
            )
            .Select(x => new SubcategoryWithFollowerCountDto
            {
                Id = x.Subcategory.Id,
                Name = x.Subcategory.Name,
                Description = x.Subcategory.Description,
                Color = x.Subcategory.Color == null ? "#6B7280" : x.Subcategory.Color,
                CreatedAt = x.Subcategory.CreatedAt,
                UpdatedAt = x.Subcategory.UpdatedAt,
                CategoryId = x.Subcategory.CategoryId,
                CategoryName = x.Subcategory.Category.Name,
                FollowerCount = x.Follows.Count(),
                IsFollowing = userId.HasValue && x.Follows.Any(f => f.UserId == userId.Value)
            })
            .FirstOrDefaultAsync();

        return result ?? new SubcategoryWithFollowerCountDto();
    }

    public async Task<int> GetSubcategoryFollowerCountAsync(int subcategoryId)
    {
        return await _context.UserSubcategoryFollows
            .CountAsync(usf => usf.SubcategoryId == subcategoryId);
    }

    public async Task<List<SubcategoryDto>> GetFollowedSubcategoriesAsync(int userId)
    {
        return await _context.UserSubcategoryFollows
            .Where(usf => usf.UserId == userId)
            .Include(usf => usf.Subcategory)
            .ThenInclude(s => s.Category)
            .Select(usf => new SubcategoryDto
            {
                Id = usf.Subcategory.Id,
                Name = usf.Subcategory.Name,
                Description = usf.Subcategory.Description,
                Color = usf.Subcategory.Color,
                CreatedAt = usf.Subcategory.CreatedAt,
                UpdatedAt = usf.Subcategory.UpdatedAt,
                CategoryId = usf.Subcategory.CategoryId,
                CategoryName = usf.Subcategory.Category.Name
            })
            .ToListAsync();
    }

    public async Task<ICollection<UserSubcategoryFollow>> GetFollowersBySubcategoryIdAsync(int subcategoryId)
    {
        return await _context.UserSubcategoryFollows
            .Where(usf => usf.SubcategoryId == subcategoryId)
            .ToListAsync();
    }
}
