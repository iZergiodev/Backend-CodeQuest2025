using CodeQuestBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeQuestBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentLikeController : ControllerBase
    {
        private readonly CommentLikeService _commentLikeService;
        private readonly ILogger<CommentLikeController> _logger;

        public CommentLikeController(CommentLikeService commentLikeService, ILogger<CommentLikeController> logger)
        {
            _commentLikeService = commentLikeService;
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

        [HttpPost("{commentId}/like")]
        public async Task<IActionResult> LikeComment(int commentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var success = await _commentLikeService.LikeCommentAsync(commentId, userId.Value);
                if (!success)
                    return BadRequest(new { message = "Comment already liked or comment not found" });

                return Ok(new { message = "Comment liked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking comment {CommentId}", commentId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{commentId}/like")]
        public async Task<IActionResult> UnlikeComment(int commentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var success = await _commentLikeService.UnlikeCommentAsync(commentId, userId.Value);
                if (!success)
                    return BadRequest(new { message = "Comment not liked or comment not found" });

                return Ok(new { message = "Comment unliked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unliking comment {CommentId}", commentId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{commentId}/likes/count")]
        public async Task<IActionResult> GetLikesCount(int commentId)
        {
            try
            {
                var count = await _commentLikeService.GetLikesCountAsync(commentId);
                return Ok(new { commentId, likesCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting likes count for comment {CommentId}", commentId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{commentId}/is-liked")]
        public async Task<IActionResult> IsLikedByUser(int commentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var isLiked = await _commentLikeService.IsLikedByUserAsync(commentId, userId.Value);
                return Ok(new { commentId, isLiked });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if comment {CommentId} is liked by user", commentId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
