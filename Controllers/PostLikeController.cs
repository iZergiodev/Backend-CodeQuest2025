using CodeQuestBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeQuestBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostLikeController : ControllerBase
    {
        private readonly PostLikeService _postLikeService;
        private readonly ILogger<PostLikeController> _logger;

        public PostLikeController(PostLikeService postLikeService, ILogger<PostLikeController> logger)
        {
            _postLikeService = postLikeService;
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

        [HttpPost("{postId}/like")]
        public async Task<IActionResult> LikePost(int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var success = await _postLikeService.LikePostAsync(postId, userId.Value);
                if (!success)
                    return BadRequest(new { message = "Post already liked or post not found" });

                return Ok(new { message = "Post liked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking post {PostId}", postId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("{postId}/toggle")]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var isLiked = await _postLikeService.IsLikedByUserAsync(postId, userId.Value);
                
                bool success;
                if (isLiked)
                {
                    success = await _postLikeService.UnlikePostAsync(postId, userId.Value);
                    return Ok(new { message = "Post unliked successfully", liked = false });
                }
                else
                {
                    success = await _postLikeService.LikePostAsync(postId, userId.Value);
                    return Ok(new { message = "Post liked successfully", liked = true });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling like for post {PostId}", postId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{postId}/like")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var success = await _postLikeService.UnlikePostAsync(postId, userId.Value);
                if (!success)
                    return BadRequest(new { message = "Post not liked or post not found" });

                return Ok(new { message = "Post unliked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unliking post {PostId}", postId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{postId}/likes/count")]
        public async Task<IActionResult> GetLikesCount(int postId)
        {
            try
            {
                var count = await _postLikeService.GetLikesCountAsync(postId);
                return Ok(new { postId, likesCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting likes count for post {PostId}", postId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{postId}/is-liked")]
        public async Task<IActionResult> IsLikedByUser(int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var isLiked = await _postLikeService.IsLikedByUserAsync(postId, userId.Value);
                return Ok(new { postId, isLiked });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if post {PostId} is liked by user", postId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
