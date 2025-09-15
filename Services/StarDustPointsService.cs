using CodeQuestBackend.Models;
using CodeQuestBackend.Repository.IRepository;

namespace CodeQuestBackend.Services;

public class StarDustPointsService
{
    private readonly IUserRepository _userRepository;
    private readonly IStarDustPointsHistoryRepository _historyRepository;
    private readonly IPostRepository _postRepository;

    public StarDustPointsService(
        IUserRepository userRepository,
        IStarDustPointsHistoryRepository historyRepository,
        IPostRepository postRepository)
    {
        _userRepository = userRepository;
        _historyRepository = historyRepository;
        _postRepository = postRepository;
    }

    public async Task AwardPointsAsync(int userId, StarDustAction action, string? description = null, int? relatedPostId = null, int? relatedCommentId = null)
    {
        var points = StarDustPoints.GetPoints(action);
        var multiplier = await CalculateMultiplierAsync(userId, relatedPostId);
        var finalPoints = (int)(points * multiplier);

        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.StarDustPoints += finalPoints;
            await _userRepository.UpdateAsync(user);
        }

        await _historyRepository.CreateAsync(new StarDustPointsHistory
        {
            UserId = userId,
            Points = finalPoints,
            Action = action.ToString(),
            Description = description ?? GetDefaultDescription(action, finalPoints),
            RelatedPostId = relatedPostId,
            RelatedCommentId = relatedCommentId,
            CreatedAt = DateTime.UtcNow
        });

        await CheckAndAwardAchievementsAsync(userId);
    }

    private async Task<double> CalculateMultiplierAsync(int userId, int? postId)
    {
        double multiplier = 1.0;

        var recentActivity = await _historyRepository.GetRecentActivityAsync(userId, 7);
        if (recentActivity.Count >= 7)
        {
            multiplier *= 1.5;
        }

        if (postId.HasValue)
        {
            var post = await _postRepository.GetByIdAsync(postId.Value);
            if (post?.CategoryId != null)
            {
                var categoryPostCount = await _postRepository.GetPostCountByCategoryAsync(post.CategoryId.Value);
                if (categoryPostCount > 50)
                {
                    multiplier *= 1.2;
                }
            }
        }

        return multiplier;
    }

    private async Task CheckAndAwardAchievementsAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return;

        var userHistory = await _historyRepository.GetByUserIdAsync(userId);

        if (!userHistory.Any(h => h.Action == StarDustAction.FirstPost.ToString()) &&
            userHistory.Any(h => h.Action == StarDustAction.PostCreated.ToString()))
        {
            await AwardPointsAsync(userId, StarDustAction.FirstPost, "¡Tu primer post!");
        }

        var postCount = userHistory.Count(h => h.Action == StarDustAction.PostCreated.ToString());
        if (postCount >= 10 && !userHistory.Any(h => h.Action == StarDustAction.TenPostsCreated.ToString()))
        {
            await AwardPointsAsync(userId, StarDustAction.TenPostsCreated, "¡10 posts creados!");
        }

        // Hundred likes received achievement
        var likesReceived = userHistory.Where(h => h.Action == StarDustAction.LikeReceived.ToString()).Sum(h => h.Points);
        if (likesReceived >= 100 && !userHistory.Any(h => h.Action == StarDustAction.HundredLikesReceived.ToString()))
        {
            await AwardPointsAsync(userId, StarDustAction.HundredLikesReceived, "¡100 likes recibidos!");
        }
    }

    public async Task OnPostCreatedAsync(int userId, int postId)
    {
        await AwardPointsAsync(userId, StarDustAction.PostCreated, "Post creado", postId);
    }

    public async Task OnPostLikedAsync(int postAuthorId, int postId, int likesCount)
    {
        await AwardPointsAsync(postAuthorId, StarDustAction.LikeReceived, "Like recibido en post", postId);

        if (likesCount == 10)
        {
            await AwardPointsAsync(postAuthorId, StarDustAction.PostReceived10Likes, "Post alcanzó 10 likes", postId);
        }
    }

    public async Task OnCommentPostedAsync(int userId, int commentId, int postId)
    {
        await AwardPointsAsync(userId, StarDustAction.CommentPosted, "Comentario publicado", postId, commentId);
    }

    public async Task OnCommentLikedAsync(int commentAuthorId, int commentId)
    {
        await AwardPointsAsync(commentAuthorId, StarDustAction.CommentLiked, "Like recibido en comentario", null, commentId);
    }

    public async Task OnPostVisitMilestoneAsync(int postAuthorId, int postId, int visitsCount)
    {
        if (visitsCount == 100)
        {
            await AwardPointsAsync(postAuthorId, StarDustAction.PostReached100Visits, "Post alcanzó 100 visitas", postId);
        }
    }

    public async Task OnCommentCountMilestoneAsync(int postAuthorId, int postId, int commentsCount)
    {
        if (commentsCount == 5)
        {
            await AwardPointsAsync(postAuthorId, StarDustAction.PostReceived5Comments, "Post alcanzó 5 comentarios", postId);
        }
    }

    public async Task<ICollection<StarDustPointsHistory>> GetUserHistoryAsync(int userId)
    {
        return await _historyRepository.GetByUserIdAsync(userId);
    }

    public async Task<ICollection<StarDustPointsHistory>> GetLeaderboardAsync(int limit = 10)
    {
        return await _historyRepository.GetTopEarnersAsync(limit);
    }

    public async Task<int> GetUserTotalPointsAsync(int userId)
    {
        return await _historyRepository.GetTotalPointsEarnedAsync(userId);
    }

    public async Task AwardCustomPointsAsync(int userId, int points, string description, int? relatedPostId = null, int? relatedCommentId = null)
    {
        // Update user points
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.StarDustPoints += points;
            await _userRepository.UpdateAsync(user);
        }

        // Record in history
        await _historyRepository.CreateAsync(new StarDustPointsHistory
        {
            UserId = userId,
            Points = points,
            Action = "Custom",
            Description = description,
            RelatedPostId = relatedPostId,
            RelatedCommentId = relatedCommentId,
            CreatedAt = DateTime.UtcNow
        });
    }

    private string GetDefaultDescription(StarDustAction action, int points)
    {
        return action switch
        {
            StarDustAction.PostCreated => $"Post creado (+{points} puntos)",
            StarDustAction.CommentPosted => $"Comentario publicado (+{points} puntos)",
            StarDustAction.LikeReceived => $"Like recibido (+{points} puntos)",
            StarDustAction.LikeGiven => $"Like dado (+{points} puntos)",
            StarDustAction.CommentLiked => $"Like en comentario (+{points} puntos)",
            StarDustAction.CommentReplied => $"Respuesta a comentario (+{points} puntos)",
            StarDustAction.PostReceived10Likes => $"Post popular (+{points} puntos)",
            StarDustAction.PostReceived5Comments => $"Post con engagement (+{points} puntos)",
            StarDustAction.PostReached100Visits => $"Post viral (+{points} puntos)",
            StarDustAction.FirstPost => $"¡Primer post! (+{points} puntos)",
            StarDustAction.TenPostsCreated => $"¡Escritor activo! (+{points} puntos)",
            StarDustAction.HundredLikesReceived => $"¡Popular en la comunidad! (+{points} puntos)",
            StarDustAction.PostOfTheDay => $"¡Post del día! (+{points} puntos)",
            _ => $"Puntos ganados (+{points})"
        };
    }
}