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
}