using CodeQuestBackend.Models;

namespace CodeQuestBackend.Repository.IRepository;

public interface ICommentLikeRepository
{
    Task<bool> LikeCommentAsync(int commentId, int userId);
    Task<bool> UnlikeCommentAsync(int commentId, int userId);
    Task<bool> IsLikedByUserAsync(int commentId, int userId);
    Task<int> GetLikesCountAsync(int commentId);
}
