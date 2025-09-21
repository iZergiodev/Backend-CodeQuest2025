using CodeQuestBackend.Repository.IRepository;

namespace CodeQuestBackend.Services;

public class CommentLikeService
{
    private readonly ICommentLikeRepository _commentLikeRepository;

    public CommentLikeService(ICommentLikeRepository commentLikeRepository)
    {
        _commentLikeRepository = commentLikeRepository;
    }

    public async Task<bool> LikeCommentAsync(int commentId, int userId)
    {
        return await _commentLikeRepository.LikeCommentAsync(commentId, userId);
    }

    public async Task<bool> UnlikeCommentAsync(int commentId, int userId)
    {
        return await _commentLikeRepository.UnlikeCommentAsync(commentId, userId);
    }

    public async Task<bool> IsLikedByUserAsync(int commentId, int userId)
    {
        return await _commentLikeRepository.IsLikedByUserAsync(commentId, userId);
    }

    public async Task<int> GetLikesCountAsync(int commentId)
    {
        return await _commentLikeRepository.GetLikesCountAsync(commentId);
    }
}
