using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeQuestBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookmarkController : ControllerBase
    {
        private readonly BookmarkService _bookmarkService;
        private readonly ILogger<BookmarkController> _logger;

        public BookmarkController(BookmarkService bookmarkService, ILogger<BookmarkController> logger)
        {
            _bookmarkService = bookmarkService;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        [HttpPost("toggle/{postId}")]
        public async Task<IActionResult> ToggleBookmark(int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var result = await _bookmarkService.ToggleBookmarkAsync(userId.Value, postId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling bookmark for post {PostId}", postId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("status/{postId}")]
        public async Task<IActionResult> GetBookmarkStatus(int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var result = await _bookmarkService.GetBookmarkStatusAsync(userId.Value, postId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookmark status for post {PostId}", postId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("my-bookmarks")]
        public async Task<IActionResult> GetMyBookmarks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var result = await _bookmarkService.GetUserBookmarksAsync(userId.Value, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user bookmarks");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("post/{postId}/bookmarks")]
        public async Task<IActionResult> GetBookmarksByPost(int postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _bookmarkService.GetBookmarksByPostAsync(postId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookmarks for post {PostId}", postId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("count/{postId}")]
        public async Task<IActionResult> GetBookmarkCount(int postId)
        {
            try
            {
                var count = await _bookmarkService.GetBookmarkCountAsync(postId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookmark count for post {PostId}", postId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
