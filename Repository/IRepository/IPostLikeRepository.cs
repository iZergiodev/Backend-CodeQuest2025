using CodeQuestBackend.Models;

namespace CodeQuestBackend.Repository.IRepository;

public interface IPostLikeRepository
{
    Task<bool> LikePostAsync(int postId, int userId);
    Task<bool> UnlikePostAsync(int postId, int userId);
    Task<bool> IsLikedByUserAsync(int postId, int userId);
    Task<int> GetLikesCountAsync(int postId);
}
