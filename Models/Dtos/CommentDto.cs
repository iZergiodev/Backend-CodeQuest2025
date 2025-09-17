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
    }

    public class CreateCommentDto
    {
        public string Content { get; set; } = string.Empty;
        public int PostId { get; set; }
    }

    public class UpdateCommentDto
    {
        public string Content { get; set; } = string.Empty;
    }
}
