using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;

namespace CodeQuestBackend.Repository.IRepository;

public interface IPostRepository
{
    Task<ICollection<Post>> GetAllAsync();
    Task<Post?> GetByIdAsync(int id);
    Task<ICollection<Post>> GetByAuthorIdAsync(int authorId);
    Task<ICollection<Post>> GetByCategoryIdAsync(int categoryId);
    Task<Post> CreateAsync(CreatePostDto createPostDto, int authorId);
    Task<Post?> UpdateAsync(int id, CreatePostDto updatePostDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task UpdateLikesCountAsync(int postId);
    Task UpdateCommentsCountAsync(int postId);
    Task IncrementVisitsCountAsync(int postId);
    Task<int> GetPostCountByCategoryAsync(int categoryId);

    // Paginated methods
    Task<PaginatedResultDto<Post>> GetAllPaginatedAsync(int page, int pageSize);
    Task<PaginatedResultDto<Post>> GetByAuthorIdPaginatedAsync(int authorId, int page, int pageSize);
    Task<PaginatedResultDto<Post>> GetByCategoryIdPaginatedAsync(int categoryId, int page, int pageSize);
    Task<PaginatedResultDto<Post>> GetBySubcategoryIdPaginatedAsync(int subcategoryId, int page, int pageSize);
    Task<PaginatedResultDto<Post>> GetByFollowedSubcategoriesAsync(List<int> subcategoryIds, int page, int pageSize, string sortBy = "recent");
}