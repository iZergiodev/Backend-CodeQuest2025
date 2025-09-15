using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeQuestBackend.Models;

public class StarDustPointsHistory
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int Points { get; set; }

    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public int? RelatedPostId { get; set; }
    public int? RelatedCommentId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("RelatedPostId")]
    public virtual Post? RelatedPost { get; set; }

    [ForeignKey("RelatedCommentId")]
    public virtual Comment? RelatedComment { get; set; }
}

public enum StarDustAction
{
    PostCreated,
    PostReceived10Likes,
    PostReceived5Comments,
    PostReached100Visits,
    CommentPosted,
    CommentLiked,
    CommentReplied,
    LikeGiven,
    LikeReceived,
    FirstPost,
    TenPostsCreated,
    HundredLikesReceived,
    PostOfTheDay
}

public static class StarDustPoints
{
    public static int GetPoints(StarDustAction action)
    {
        return action switch
        {
            StarDustAction.PostCreated => 10,
            StarDustAction.PostReceived10Likes => 5,
            StarDustAction.PostReceived5Comments => 8,
            StarDustAction.PostReached100Visits => 15,
            StarDustAction.CommentPosted => 2,
            StarDustAction.CommentLiked => 1,
            StarDustAction.CommentReplied => 1,
            StarDustAction.LikeGiven => 1,
            StarDustAction.LikeReceived => 1,
            StarDustAction.FirstPost => 20,
            StarDustAction.TenPostsCreated => 50,
            StarDustAction.HundredLikesReceived => 100,
            StarDustAction.PostOfTheDay => 200,
            _ => 0
        };
    }
}