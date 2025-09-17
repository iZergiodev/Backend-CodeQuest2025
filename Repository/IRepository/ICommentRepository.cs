using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;

namespace CodeQuestBackend.Repository.IRepository
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(int id);
        Task<IEnumerable<CommentDto>> GetByPostIdAsync(int postId);
        Task<IEnumerable<CommentDto>> GetByAuthorIdAsync(int authorId);
        Task<Comment> CreateAsync(CreateCommentDto createCommentDto, int authorId);
        Task<Comment?> UpdateAsync(int id, UpdateCommentDto updateCommentDto, int authorId);
        Task<bool> DeleteAsync(int id, int authorId);
        Task<int> GetCountByAuthorIdAsync(int authorId);
    }
}
