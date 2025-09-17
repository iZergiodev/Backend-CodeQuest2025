using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;

namespace CodeQuestBackend.Repository.IRepository
{
    public interface IBookmarkRepository
    {
        Task<Bookmark?> GetBookmarkAsync(int userId, int postId);
        Task<Bookmark> CreateBookmarkAsync(int userId, int postId);
        Task<bool> DeleteBookmarkAsync(int userId, int postId);
        Task<bool> IsBookmarkedAsync(int userId, int postId);
        Task<int> GetBookmarkCountAsync(int postId);
        Task<List<BookmarkDto>> GetUserBookmarksAsync(int userId, int page = 1, int pageSize = 10);
        Task<int> GetUserBookmarkCountAsync(int userId);
        Task<List<BookmarkDto>> GetBookmarksByPostAsync(int postId, int page = 1, int pageSize = 10);
    }
}
