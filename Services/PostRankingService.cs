using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;

namespace CodeQuestBackend.Services;

public class PostRankingService
{
    public class RankingConfig
    {
        public double LikesWeight { get; set; } = 3.0;
        public double CommentsWeight { get; set; } = 5.0;
        public double VisitsWeight { get; set; } = 1.0;
        public double StarDustPointsWeight { get; set; } = 2.0;
        public double TimeDecayFactor { get; set; } = 0.1;
    }

    private readonly RankingConfig _config;

    public PostRankingService(RankingConfig? config = null)
    {
        _config = config ?? new RankingConfig();
    }

    public double CalculatePostScore(PostDto post, int authorStarDustPoints)
    {
        var engagementScore = CalculateEngagementScore(post);
        var authorScore = CalculateAuthorScore(authorStarDustPoints);
        var timeScore = CalculateTimeScore(post.CreatedAt);

        return engagementScore + authorScore + timeScore;
    }

    private double CalculateEngagementScore(PostDto post)
    {
        var likesScore = post.LikesCount * _config.LikesWeight;
        var commentsScore = post.CommentsCount * _config.CommentsWeight;
        var visitsScore = post.VisitsCount * _config.VisitsWeight;

        return likesScore + commentsScore + visitsScore;
    }

    private double CalculateAuthorScore(int starDustPoints)
    {
        return Math.Sqrt(starDustPoints) * _config.StarDustPointsWeight;
    }

    private double CalculateTimeScore(DateTime createdAt)
    {
        var daysSinceCreation = (DateTime.UtcNow - createdAt).TotalDays;
        return Math.Max(0, 100 - (daysSinceCreation * _config.TimeDecayFactor));
    }

    public IEnumerable<PostDto> RankPosts(IEnumerable<PostDto> posts, Dictionary<int, int> authorStarDustPoints)
    {
        return posts
            .Select(post => new
            {
                Post = post,
                Score = CalculatePostScore(post, authorStarDustPoints.GetValueOrDefault(post.AuthorId, 0))
            })
            .OrderByDescending(x => x.Score)
            .Select(x => x.Post);
    }
}