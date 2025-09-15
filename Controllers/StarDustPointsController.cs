using CodeQuestBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeQuestBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StarDustPointsController : ControllerBase
{
    private readonly StarDustPointsService _starDustPointsService;

    public StarDustPointsController(StarDustPointsService starDustPointsService)
    {
        _starDustPointsService = starDustPointsService;
    }

    [HttpGet("history/{userId:int}")]
    public async Task<IActionResult> GetUserPointsHistory(int userId)
    {
        try
        {
            var history = await _starDustPointsService.GetUserHistoryAsync(userId);
            return Ok(history);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener el historial de puntos: {ex.Message}");
        }
    }

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard([FromQuery] int limit = 10)
    {
        try
        {
            var leaderboard = await _starDustPointsService.GetLeaderboardAsync(limit);
            return Ok(leaderboard);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener el ranking: {ex.Message}");
        }
    }

    [HttpGet("total/{userId:int}")]
    public async Task<IActionResult> GetUserTotalPoints(int userId)
    {
        try
        {
            var totalPoints = await _starDustPointsService.GetUserTotalPointsAsync(userId);
            return Ok(new { userId, totalPoints });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los puntos totales: {ex.Message}");
        }
    }

    [HttpPost("award")]
    public async Task<IActionResult> AwardPoints([FromBody] AwardPointsRequest request)
    {
        try
        {
            await _starDustPointsService.AwardCustomPointsAsync(
                request.UserId,
                request.Points,
                request.Description,
                request.RelatedPostId,
                request.RelatedCommentId
            );

            return Ok(new { message = "Puntos otorgados correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al otorgar puntos: {ex.Message}");
        }
    }
}

public class AwardPointsRequest
{
    public int UserId { get; set; }
    public int Points { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? RelatedPostId { get; set; }
    public int? RelatedCommentId { get; set; }
}