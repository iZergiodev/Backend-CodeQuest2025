namespace CodeQuestBackend.Models.Dtos;

public class NotificationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public int? RelatedPostId { get; set; }
    public string? RelatedPostTitle { get; set; }
    public int? RelatedCommentId { get; set; }
    public int? RelatedUserId { get; set; }
    public string? RelatedUserName { get; set; }
    public string? RelatedUserAvatar { get; set; }
    public DateTime CreatedAt { get; set; }
}