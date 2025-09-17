using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeQuestBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserFollowController : ControllerBase
{
    private readonly UserFollowService _userFollowService;
    private readonly ILogger<UserFollowController> _logger;

    public UserFollowController(UserFollowService userFollowService, ILogger<UserFollowController> logger)
    {
        _userFollowService = userFollowService;
        _logger = logger;
    }

    [HttpPost("category/all-subcategories")]
    public async Task<IActionResult> FollowAllSubcategories([FromBody] FollowAllSubcategoriesDto followDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _userFollowService.FollowAllSubcategoriesAsync(userId.Value, followDto.CategoryId);
            
            if (result)
                return Ok(new { message = "All subcategories followed successfully" });
            
            return BadRequest(new { message = "Category not found or no subcategories available" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following all subcategories");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("subcategory")]
    public async Task<IActionResult> FollowSubcategory([FromBody] FollowSubcategoryDto followDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _userFollowService.FollowSubcategoryAsync(userId.Value, followDto.SubcategoryId);
            
            if (result)
                return Ok(new { message = "Subcategory followed successfully" });
            
            return BadRequest(new { message = "Already following this subcategory or subcategory not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following subcategory");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("subcategory/{subcategoryId}")]
    public async Task<IActionResult> UnfollowSubcategory(int subcategoryId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _userFollowService.UnfollowSubcategoryAsync(userId.Value, subcategoryId);
            
            if (result)
                return Ok(new { message = "Subcategory unfollowed successfully" });
            
            return BadRequest(new { message = "Not following this subcategory" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfollowing subcategory");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }


    [HttpGet("status/subcategory/{subcategoryId}")]
    public async Task<IActionResult> IsFollowingSubcategory(int subcategoryId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var isFollowing = await _userFollowService.IsFollowingSubcategoryAsync(userId.Value, subcategoryId);
            return Ok(new { isFollowing });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking subcategory follow status");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("my-follows")]
    public async Task<IActionResult> GetMyFollows()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var follows = await _userFollowService.GetUserFollowsAsync(userId.Value);
            return Ok(follows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user follows");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("subcategories/with-count")]
    public async Task<IActionResult> GetSubcategoriesWithFollowerCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            var subcategories = await _userFollowService.GetSubcategoriesWithFollowerCountAsync(userId);
            return Ok(subcategories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subcategories with follower count");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("subcategory/{subcategoryId}/with-count")]
    public async Task<IActionResult> GetSubcategoryWithFollowerCount(int subcategoryId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var subcategory = await _userFollowService.GetSubcategoryWithFollowerCountAsync(subcategoryId, userId);
            return Ok(subcategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subcategory with follower count");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("subcategory/{subcategoryId}/follower-count")]
    public async Task<IActionResult> GetSubcategoryFollowerCount(int subcategoryId)
    {
        try
        {
            var count = await _userFollowService.GetSubcategoryFollowerCountAsync(subcategoryId);
            return Ok(new { followerCount = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subcategory follower count");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("subcategories")]
    public async Task<IActionResult> GetFollowedSubcategories()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var subcategories = await _userFollowService.GetFollowedSubcategoriesAsync(userId.Value);
            return Ok(subcategories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followed subcategories");
            return StatusCode(500, new { message = "Internal server error" });
        }
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
}
