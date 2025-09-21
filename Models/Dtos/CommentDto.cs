using System;

namespace CodeQuestBackend.Models.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int PostId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public int? ParentId { get; set; }
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
        public int LikesCount { get; set; } = 0;
        public int RepliesCount { get; set; } = 0;
        public bool IsLikedByUser { get; set; } = false;
    }

    public class CreateCommentDto
    {
        public string Content { get; set; } = string.Empty;
        public int PostId { get; set; }
        public int? ParentId { get; set; }
    }

    public class UpdateCommentDto
    {
        public string Content { get; set; } = string.Empty;
    }
}
