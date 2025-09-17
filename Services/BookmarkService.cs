using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;

namespace CodeQuestBackend.Services
{
    public class BookmarkService
    {
        private readonly IBookmarkRepository _bookmarkRepository;

        public BookmarkService(IBookmarkRepository bookmarkRepository)
        {
            _bookmarkRepository = bookmarkRepository;
        }

        public async Task<BookmarkResponseDto> ToggleBookmarkAsync(int userId, int postId)
        {
            var isBookmarked = await _bookmarkRepository.IsBookmarkedAsync(userId, postId);

            if (isBookmarked)
            {
                await _bookmarkRepository.DeleteBookmarkAsync(userId, postId);
            }
            else
            {
                await _bookmarkRepository.CreateBookmarkAsync(userId, postId);
            }

            var newBookmarkCount = await _bookmarkRepository.GetBookmarkCountAsync(postId);
            var newIsBookmarked = !isBookmarked;

            return new BookmarkResponseDto
            {
                IsBookmarked = newIsBookmarked,
                BookmarkCount = newBookmarkCount
            };
        }

        public async Task<BookmarkResponseDto> GetBookmarkStatusAsync(int userId, int postId)
        {
            var isBookmarked = await _bookmarkRepository.IsBookmarkedAsync(userId, postId);
            var bookmarkCount = await _bookmarkRepository.GetBookmarkCountAsync(postId);

            return new BookmarkResponseDto
            {
                IsBookmarked = isBookmarked,
                BookmarkCount = bookmarkCount
            };
        }

        public async Task<UserBookmarksDto> GetUserBookmarksAsync(int userId, int page = 1, int pageSize = 10)
        {
            var bookmarks = await _bookmarkRepository.GetUserBookmarksAsync(userId, page, pageSize);
            var totalCount = await _bookmarkRepository.GetUserBookmarkCountAsync(userId);

            return new UserBookmarksDto
            {
                Bookmarks = bookmarks,
                TotalCount = totalCount
            };
        }

        public async Task<List<BookmarkDto>> GetBookmarksByPostAsync(int postId, int page = 1, int pageSize = 10)
        {
            return await _bookmarkRepository.GetBookmarksByPostAsync(postId, page, pageSize);
        }

        public async Task<bool> IsBookmarkedAsync(int userId, int postId)
        {
            return await _bookmarkRepository.IsBookmarkedAsync(userId, postId);
        }

        public async Task<int> GetBookmarkCountAsync(int postId)
        {
            return await _bookmarkRepository.GetBookmarkCountAsync(postId);
        }
    }
}
