using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;

namespace CodeQuestBackend.Services
{
    public class CommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
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
                AuthorAvatar = comment.Author.Avatar
            };
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId)
        {
            return await _commentRepository.GetByPostIdAsync(postId);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByAuthorIdAsync(int authorId)
        {
            return await _commentRepository.GetByAuthorIdAsync(authorId);
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto, int authorId)
        {
            var comment = await _commentRepository.CreateAsync(createCommentDto, authorId);
            
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
