using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Services;
using CodeQuestBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeQuestBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly CommentService _commentService;
        private readonly TrendingService _trendingService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(CommentService commentService, TrendingService trendingService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _trendingService = trendingService;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            try
            {
                var comment = await _commentService.GetCommentByIdAsync(id);
                if (comment == null)
                    return NotFound();

                return Ok(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment {CommentId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentsByPost(int postId, [FromQuery] string sortBy = "newest")
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var comments = await _commentService.GetCommentsByPostIdAsync(postId, currentUserId, sortBy);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for post {PostId}", postId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetCommentsByAuthor(int authorId)
        {
            try
            {
                var comments = await _commentService.GetCommentsByAuthorIdAsync(authorId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for author {AuthorId}", authorId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var comment = await _commentService.CreateCommentAsync(createCommentDto, userId.Value);
                
                // Record engagement for trending
                await _trendingService.RecordEngagementAsync(createCommentDto.PostId, userId.Value, EngagementType.Comment);
                
                return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDto updateCommentDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var comment = await _commentService.UpdateCommentAsync(id, updateCommentDto, userId.Value);
                if (comment == null)
                    return NotFound();

                return Ok(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment {CommentId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var success = await _commentService.DeleteCommentAsync(id, userId.Value);
                if (!success)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment {CommentId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("author/{authorId}/count")]
        public async Task<IActionResult> GetCommentCountByAuthor(int authorId)
        {
            try
            {
                var count = await _commentService.GetCommentCountByAuthorIdAsync(authorId);
                return Ok(new { authorId, count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment count for author {AuthorId}", authorId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
