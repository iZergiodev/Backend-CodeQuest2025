namespace CodeQuestBackend.Models.Dtos
{
    public class BookmarkDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public DateTime CreatedAt { get; set; }
        public PostDto? Post { get; set; }
    }

    public class CreateBookmarkDto
    {
        public int PostId { get; set; }
    }

    public class BookmarkResponseDto
    {
        public bool IsBookmarked { get; set; }
        public int BookmarkCount { get; set; }
    }

    public class UserBookmarksDto
    {
        public List<BookmarkDto> Bookmarks { get; set; } = new List<BookmarkDto>();
        public int TotalCount { get; set; }
    }
}
