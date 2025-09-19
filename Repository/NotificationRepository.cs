using CodeQuestBackend.Data;
using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Repository;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _db;

    public NotificationRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ICollection<Notification>> GetByUserIdAsync(int userId)
    {
        return await _db.Notifications
            .Include(n => n.RelatedPost)
            .Include(n => n.RelatedComment)
            .Include(n => n.RelatedUser)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<ICollection<Notification>> GetUnreadByUserIdAsync(int userId)
    {
        return await _db.Notifications
            .Include(n => n.RelatedPost)
            .Include(n => n.RelatedComment)
            .Include(n => n.RelatedUser)
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountByUserIdAsync(int userId)
    {
        return await _db.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _db.Notifications
            .Include(n => n.RelatedPost)
            .Include(n => n.RelatedComment)
            .Include(n => n.RelatedUser)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<Notification> CreateAsync(CreateNotificationDto createNotificationDto)
    {
        var notification = new Notification
        {
            UserId = createNotificationDto.UserId,
            Type = createNotificationDto.Type,
            Title = createNotificationDto.Title,
            Message = createNotificationDto.Message,
            RelatedPostId = createNotificationDto.RelatedPostId,
            RelatedCommentId = createNotificationDto.RelatedCommentId,
            RelatedUserId = createNotificationDto.RelatedUserId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(notification.Id) ?? notification;
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var notification = await _db.Notifications.FindAsync(id);
        if (notification == null)
            return false;

        notification.IsRead = true;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(int userId)
    {
        var notifications = await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var notification = await _db.Notifications.FindAsync(id);
        if (notification == null)
            return false;

        _db.Notifications.Remove(notification);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _db.Notifications.AnyAsync(n => n.Id == id);
    }
}