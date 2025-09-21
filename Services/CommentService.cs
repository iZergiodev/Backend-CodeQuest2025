using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;

namespace CodeQuestBackend.Services
{
    public class CommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly NotificationService _notificationService;

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository, NotificationService notificationService)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _notificationService = notificationService;
        }

        public async Task<CommentDto?> GetCommentByIdAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                return null;

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                PostId = comment.PostId,
                PostTitle = comment.Post.Title,
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author.Name ?? comment.Author.Username ?? "Unknown",
                AuthorAvatar = comment.Author.Avatar,
                ParentId = comment.ParentId,
                Replies = new List<CommentDto>() // For individual comment retrieval, we don't load replies
            };
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId)
        {
            return await _commentRepository.GetByPostIdAsync(postId);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId, int? currentUserId)
        {
            return await _commentRepository.GetByPostIdAsync(postId, currentUserId);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId, int? currentUserId, string sortBy)
        {
            return await _commentRepository.GetByPostIdAsync(postId, currentUserId, sortBy);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByAuthorIdAsync(int authorId)
        {
            return await _commentRepository.GetByAuthorIdAsync(authorId);
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto, int authorId)
        {
            var comment = await _commentRepository.CreateAsync(createCommentDto, authorId);

            // Get post information for notification
            var post = await _postRepository.GetByIdAsync(createCommentDto.PostId);
            if (post != null && post.AuthorId != authorId)
            {
                // Create notification for post author
                await _notificationService.CreateCommentNotificationAsync(
                    post.AuthorId,
                    authorId,
                    post.Id,
                    comment.Id
                );
            }

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                PostId = comment.PostId,
                PostTitle = comment.Post?.Title ?? "Unknown Post",
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author?.Name ?? comment.Author?.Username ?? "Unknown",
                AuthorAvatar = comment.Author?.Avatar
            };
        }

        public async Task<CommentDto?> UpdateCommentAsync(int id, UpdateCommentDto updateCommentDto, int authorId)
        {
            var comment = await _commentRepository.UpdateAsync(id, updateCommentDto, authorId);
            if (comment == null)
                return null;

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                PostId = comment.PostId,
                PostTitle = comment.Post?.Title ?? "Unknown Post",
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author?.Name ?? comment.Author?.Username ?? "Unknown",
                AuthorAvatar = comment.Author?.Avatar
            };
        }

        public async Task<bool> DeleteCommentAsync(int id, int authorId)
        {
            return await _commentRepository.DeleteAsync(id, authorId);
        }

        public async Task<int> GetCommentCountByAuthorIdAsync(int authorId)
        {
            return await _commentRepository.GetCountByAuthorIdAsync(authorId);
        }
    }
}
