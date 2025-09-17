using System.ComponentModel.DataAnnotations;

namespace CodeQuestBackend.Models.Dtos;

public class CreateNotificationDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = string.Empty;

    public int? RelatedPostId { get; set; }
    public int? RelatedCommentId { get; set; }
    public int? RelatedUserId { get; set; }
}