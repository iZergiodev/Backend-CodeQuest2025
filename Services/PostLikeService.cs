using CodeQuestBackend.Repository.IRepository;

namespace CodeQuestBackend.Services;

public class PostLikeService
{
    private readonly IPostLikeRepository _postLikeRepository;

    public PostLikeService(IPostLikeRepository postLikeRepository)
    {
        _postLikeRepository = postLikeRepository;
    }

    public async Task<bool> LikePostAsync(int postId, int userId)
    {
        return await _postLikeRepository.LikePostAsync(postId, userId);
    }

    public async Task<bool> UnlikePostAsync(int postId, int userId)
    {
        return await _postLikeRepository.UnlikePostAsync(postId, userId);
    }

    public async Task<bool> IsLikedByUserAsync(int postId, int userId)
    {
        return await _postLikeRepository.IsLikedByUserAsync(postId, userId);
    }

    public async Task<int> GetLikesCountAsync(int postId)
    {
        return await _postLikeRepository.GetLikesCountAsync(postId);
    }
}
