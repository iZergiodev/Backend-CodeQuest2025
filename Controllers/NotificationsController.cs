using CodeQuestBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeQuestBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly NotificationService _notificationService;
    private readonly NotificationStreamService _streamService;

    public NotificationsController(NotificationService notificationService, NotificationStreamService streamService)
    {
        _notificationService = notificationService;
        _streamService = streamService;
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetNotificationsByUserId(int userId)
    {
        try
        {
            var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las notificaciones: {ex.Message}");
        }
    }

    [HttpGet("user/{userId:int}/unread")]
    public async Task<IActionResult> GetUnreadNotificationsByUserId(int userId)
    {
        try
        {
            var notifications = await _notificationService.GetUnreadNotificationsByUserIdAsync(userId);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las notificaciones no leídas: {ex.Message}");
        }
    }

    [HttpGet("user/{userId:int}/unread/count")]
    public async Task<IActionResult> GetUnreadCount(int userId)
    {
        try
        {
            var count = await _notificationService.GetUnreadCountByUserIdAsync(userId);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener el contador de notificaciones no leídas: {ex.Message}");
        }
    }

    [HttpGet("{id:int}", Name = "GetNotification")]
    public async Task<IActionResult> GetNotificationById(int id)
    {
        try
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound($"La notificación con ID {id} no existe");
            }
            return Ok(notification);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener la notificación: {ex.Message}");
        }
    }

    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            var success = await _notificationService.MarkAsReadAsync(id);
            if (!success)
            {
                return NotFound($"La notificación con ID {id} no existe");
            }
            return Ok(new { message = "Notificación marcada como leída" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al marcar la notificación como leída: {ex.Message}");
        }
    }

    [HttpPut("user/{userId:int}/read-all")]
    public async Task<IActionResult> MarkAllAsRead(int userId)
    {
        try
        {
            var success = await _notificationService.MarkAllAsReadAsync(userId);
            if (!success)
            {
                return BadRequest("Error al marcar todas las notificaciones como leídas");
            }
            return Ok(new { message = "Todas las notificaciones marcadas como leídas" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al marcar todas las notificaciones como leídas: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        try
        {
            var success = await _notificationService.DeleteNotificationAsync(id);
            if (!success)
            {
                return NotFound($"La notificación con ID {id} no existe");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar la notificación: {ex.Message}");
        }
    }

    [HttpGet("user/{userId:int}/stream")]
    public async Task StreamNotifications(int userId, CancellationToken cancellationToken)
    {
        Response.Headers["Content-Type"] = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["Connection"] = "keep-alive";
        Response.Headers["Access-Control-Allow-Origin"] = "*";
        Response.Headers["Access-Control-Allow-Headers"] = "Cache-Control";

        var writer = new StreamWriter(Response.Body);

        try
        {
            _streamService.AddConnection(userId, writer);

            // Send initial unread count
            var unreadCount = await _notificationService.GetUnreadCountByUserIdAsync(userId);
            await _streamService.SendUnreadCountToUserAsync(userId, unreadCount);

            // Keep connection alive
            while (!cancellationToken.IsCancellationRequested)
            {
                // Send heartbeat every 30 seconds
                await writer.WriteAsync(": heartbeat\n\n");
                await writer.FlushAsync();

                await Task.Delay(30000, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Client disconnected, this is normal
        }
        catch (Exception ex)
        {
            // Log error but don't throw
            Console.WriteLine($"SSE Error for user {userId}: {ex.Message}");
        }
        finally
        {
            _streamService.RemoveConnection(userId, writer);
            writer?.Dispose();
        }
    }
}