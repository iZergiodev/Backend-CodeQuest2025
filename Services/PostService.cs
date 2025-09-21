using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;

namespace CodeQuestBackend.Services;

public class PostService
{
    private readonly IPostRepository _postRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly PostRankingService _rankingService;
    private readonly StarDustPointsService _starDustPointsService;
    private readonly NotificationService _notificationService;

    public PostService(IPostRepository postRepository, ICategoryRepository categoryRepository, IUserRepository userRepository, PostRankingService rankingService, StarDustPointsService starDustPointsService, NotificationService notificationService)
    {
        _postRepository = postRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _rankingService = rankingService;
        _starDustPointsService = starDustPointsService;
        _notificationService = notificationService;
    }

    public async Task<ICollection<PostDto>> GetAllPostsAsync()
    {
        var posts = await _postRepository.GetAllAsync();
        return posts.Select(post => MapToPostDto(post)).ToList();
    }

    public async Task<PostDto?> GetPostByIdAsync(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return post != null ? MapToPostDto(post) : null;
    }

    public async Task<PostDto?> GetPostByIdAsync(int id, int? currentUserId)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return post != null ? MapToPostDto(post, currentUserId) : null;
    }

    public async Task<ICollection<PostDto>> GetPostsByAuthorIdAsync(int authorId)
    {
        var posts = await _postRepository.GetByAuthorIdAsync(authorId);
        return posts.Select(post => MapToPostDto(post)).ToList();
    }

    public async Task<ICollection<PostDto>> GetPostsByCategoryIdAsync(int categoryId)
    {
        var posts = await _postRepository.GetByCategoryIdAsync(categoryId);
        return posts.Select(post => MapToPostDto(post)).ToList();
    }

    public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto, int authorId)
    {
        // Validate author exists
        if (!await _userRepository.ExistsAsync(authorId))
        {
            throw new InvalidOperationException($"User with ID {authorId} does not exist.");
        }

        if (createPostDto.CategoryId.HasValue)
        {
            if (!await _categoryRepository.ExistsAsync(createPostDto.CategoryId.Value))
            {
                throw new InvalidOperationException($"Category with ID {createPostDto.CategoryId} does not exist.");
            }
        }

        var post = await _postRepository.CreateAsync(createPostDto, authorId);

        // Award points for creating a post
        await _starDustPointsService.OnPostCreatedAsync(authorId, post.Id);

        // Create notifications for followers of the subcategory
        if (post.SubcategoryId.HasValue)
        {
            await _notificationService.CreatePostInFollowedSubcategoryNotificationAsync(
                authorId,
                post.Id,
                post.SubcategoryId.Value
            );
        }

        return MapToPostDto(post);
    }

    public async Task<PostDto?> UpdatePostAsync(int id, CreatePostDto updatePostDto)
    {
        if (!await _postRepository.ExistsAsync(id))
        {
            return null;
        }

        if (updatePostDto.CategoryId.HasValue)
        {
            if (!await _categoryRepository.ExistsAsync(updatePostDto.CategoryId.Value))
            {
                throw new InvalidOperationException($"Category with ID {updatePostDto.CategoryId} does not exist.");
            }
        }

        var updatedPost = await _postRepository.UpdateAsync(id, updatePostDto);
        return updatedPost != null ? MapToPostDto(updatedPost) : null;
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        return await _postRepository.DeleteAsync(id);
    }

    public async Task<bool> PostExistsAsync(int id)
    {
        return await _postRepository.ExistsAsync(id);
    }

    public async Task<bool> IncrementVisitsAsync(int id)
    {
        if (!await _postRepository.ExistsAsync(id))
        {
            return false;
        }

        await _postRepository.IncrementVisitsCountAsync(id);

        // Check for visit milestones and award points
        var post = await _postRepository.GetByIdAsync(id);
        if (post != null)
        {
            await _starDustPointsService.OnPostVisitMilestoneAsync(post.AuthorId, id, post.VisitsCount);
        }

        return true;
    }

    public async Task<ICollection<PostDto>> GetRankedPostsAsync()
    {
        var posts = await _postRepository.GetAllAsync();
        var postDtos = posts.Select(post => MapToPostDto(post)).ToList();

        var authorIds = postDtos.Select(p => p.AuthorId).Distinct().ToList();
        var authors = await _userRepository.GetUsersByIdsAsync(authorIds);
        var authorStarDustPoints = authors.ToDictionary(u => u.Id, u => u.StarDustPoints);

        var rankedPosts = _rankingService.RankPosts(postDtos, authorStarDustPoints);
        return rankedPosts.ToList();
    }

    public async Task<ICollection<PostDto>> GetRankedPostsByCategoryAsync(int categoryId)
    {
        var posts = await _postRepository.GetByCategoryIdAsync(categoryId);
        var postDtos = posts.Select(post => MapToPostDto(post)).ToList();

        var authorIds = postDtos.Select(p => p.AuthorId).Distinct().ToList();
        var authors = await _userRepository.GetUsersByIdsAsync(authorIds);
        var authorStarDustPoints = authors.ToDictionary(u => u.Id, u => u.StarDustPoints);

        var rankedPosts = _rankingService.RankPosts(postDtos, authorStarDustPoints);
        return rankedPosts.ToList();
    }

    // Paginated methods
    public async Task<PaginatedResultDto<PostDto>> GetAllPostsPaginatedAsync(int page, int pageSize)
    {
        var result = await _postRepository.GetAllPaginatedAsync(page, pageSize);

        return new PaginatedResultDto<PostDto>
        {
            Data = result.Data.Select(post => MapToPostDto(post)).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };
    }

    public async Task<PaginatedResultDto<PostDto>> GetPostsByAuthorIdPaginatedAsync(int authorId, int page, int pageSize)
    {
        var result = await _postRepository.GetByAuthorIdPaginatedAsync(authorId, page, pageSize);

        return new PaginatedResultDto<PostDto>
        {
            Data = result.Data.Select(post => MapToPostDto(post)).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };
    }

    public async Task<PaginatedResultDto<PostDto>> GetPostsByCategoryIdPaginatedAsync(int categoryId, int page, int pageSize)
    {
        var result = await _postRepository.GetByCategoryIdPaginatedAsync(categoryId, page, pageSize);

        return new PaginatedResultDto<PostDto>
        {
            Data = result.Data.Select(post => MapToPostDto(post)).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };
    }

    public async Task<PaginatedResultDto<PostDto>> GetPostsBySubcategoryIdPaginatedAsync(int subcategoryId, int page, int pageSize)
    {
        var result = await _postRepository.GetBySubcategoryIdPaginatedAsync(subcategoryId, page, pageSize);

        return new PaginatedResultDto<PostDto>
        {
            Data = result.Data.Select(post => MapToPostDto(post)).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };
    }

    public async Task<PaginatedResultDto<PostDto>> GetRankedPostsPaginatedAsync(int page, int pageSize)
    {
        var result = await _postRepository.GetAllPaginatedAsync(page, pageSize);
        var postDtos = result.Data.Select(post => MapToPostDto(post)).ToList();

        var authorIds = postDtos.Select(p => p.AuthorId).Distinct().ToList();
        var authors = await _userRepository.GetUsersByIdsAsync(authorIds);
        var authorStarDustPoints = authors.ToDictionary(u => u.Id, u => u.StarDustPoints);

        var rankedPosts = _rankingService.RankPosts(postDtos, authorStarDustPoints);

        return new PaginatedResultDto<PostDto>
        {
            Data = rankedPosts.ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };
    }

    public async Task<PaginatedResultDto<PostDto>> GetRankedPostsByCategoryPaginatedAsync(int categoryId, int page, int pageSize)
    {
        var result = await _postRepository.GetByCategoryIdPaginatedAsync(categoryId, page, pageSize);
        var postDtos = result.Data.Select(post => MapToPostDto(post)).ToList();

        var authorIds = postDtos.Select(p => p.AuthorId).Distinct().ToList();
        var authors = await _userRepository.GetUsersByIdsAsync(authorIds);
        var authorStarDustPoints = authors.ToDictionary(u => u.Id, u => u.StarDustPoints);

        var rankedPosts = _rankingService.RankPosts(postDtos, authorStarDustPoints);

        return new PaginatedResultDto<PostDto>
        {
            Data = rankedPosts.ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };
    }

    private static PostDto MapToPostDto(Post post, int? currentUserId = null)
    {
        var isLikedByUser = currentUserId.HasValue && 
                           post.Likes.Any(l => l.UserId == currentUserId.Value);

        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Summary = post.Summary,
            ImageUrl = post.ImageUrl,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            AuthorId = post.AuthorId,
            AuthorAvatar = post.Author?.Avatar,
            AuthorName = post.Author?.Name ?? string.Empty,
            CategoryId = post.CategoryId,
            CategoryName = post.Category?.Name,
            CategoryColor = post.Category?.Color,
            SubcategoryId = post.SubcategoryId,
            SubcategoryName = post.Subcategory?.Name,
            SubcategoryColor = post.Subcategory?.Color,
            Tags = post.Tags,
            LikesCount = post.LikesCount,
            CommentsCount = post.CommentsCount,
            VisitsCount = post.VisitsCount,
            IsLikedByUser = isLikedByUser
        };
    }

    public async Task<PaginatedResultDto<PostDto>> GetPostsByFollowedSubcategoriesAsync(List<int> subcategoryIds, int page, int pageSize, string sortBy = "recent")
    {
        var result = await _postRepository.GetByFollowedSubcategoriesAsync(subcategoryIds, page, pageSize, sortBy);

        return new PaginatedResultDto<PostDto>
        {
            Data = result.Data.Select(post => MapToPostDto(post)).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };
    }

    public async Task<PaginatedResultDto<PostDto>> SearchPostsAsync(string query, int page, int pageSize, string sortBy = "recent", int? currentUserId = null)
    {
        var result = await _postRepository.SearchPostsAsync(query, page, pageSize, sortBy);

        return new PaginatedResultDto<PostDto>
        {
            Data = result.Data.Select(post => MapToPostDto(post, currentUserId)).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };
    }
}