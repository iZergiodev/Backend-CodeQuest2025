using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;

namespace CodeQuestBackend.Services;

public class PostService
{
    private readonly IPostRepository _postRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;

    public PostService(IPostRepository postRepository, ICategoryRepository categoryRepository, IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
    }

    public async Task<ICollection<PostDto>> GetAllPostsAsync()
    {
        var posts = await _postRepository.GetAllAsync();
        return posts.Select(MapToPostDto).ToList();
    }

    public async Task<PostDto?> GetPostByIdAsync(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return post != null ? MapToPostDto(post) : null;
    }

    public async Task<ICollection<PostDto>> GetPostsByAuthorIdAsync(int authorId)
    {
        var posts = await _postRepository.GetByAuthorIdAsync(authorId);
        return posts.Select(MapToPostDto).ToList();
    }

    public async Task<ICollection<PostDto>> GetPostsByCategoryIdAsync(int categoryId)
    {
        var posts = await _postRepository.GetByCategoryIdAsync(categoryId);
        return posts.Select(MapToPostDto).ToList();
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

    private static PostDto MapToPostDto(Post post)
    {
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
            AuthorName = post.Author?.Name ?? string.Empty,
            CategoryId = post.CategoryId,
            CategoryName = post.Category?.Name,
            LikesCount = post.LikesCount,
            CommentsCount = post.CommentsCount
        };
    }
}