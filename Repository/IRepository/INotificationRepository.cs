using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;

namespace CodeQuestBackend.Repository.IRepository;

public interface INotificationRepository
{
    Task<ICollection<Notification>> GetByUserIdAsync(int userId);
    Task<ICollection<Notification>> GetUnreadByUserIdAsync(int userId);
    Task<int> GetUnreadCountByUserIdAsync(int userId);
    Task<Notification?> GetByIdAsync(int id);
    Task<Notification> CreateAsync(CreateNotificationDto createNotificationDto);
    Task<bool> MarkAsReadAsync(int id);
    Task<bool> MarkAllAsReadAsync(int userId);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}