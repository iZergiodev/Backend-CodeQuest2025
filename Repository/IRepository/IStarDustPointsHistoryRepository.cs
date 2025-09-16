using CodeQuestBackend.Models;

namespace CodeQuestBackend.Repository.IRepository;

public interface IStarDustPointsHistoryRepository
{
    Task<StarDustPointsHistory> CreateAsync(StarDustPointsHistory history);
    Task<ICollection<StarDustPointsHistory>> GetByUserIdAsync(int userId);
    Task<ICollection<StarDustPointsHistory>> GetRecentActivityAsync(int userId, int days);
    Task<ICollection<StarDustPointsHistory>> GetTopEarnersAsync(int limit = 10);
    Task<int> GetTotalPointsEarnedAsync(int userId);
}