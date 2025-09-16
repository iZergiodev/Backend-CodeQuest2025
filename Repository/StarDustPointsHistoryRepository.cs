using CodeQuestBackend.Data;
using CodeQuestBackend.Models;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Repository;

public class StarDustPointsHistoryRepository : IStarDustPointsHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public StarDustPointsHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StarDustPointsHistory> CreateAsync(StarDustPointsHistory history)
    {
        _context.StarDustPointsHistory.Add(history);
        await _context.SaveChangesAsync();
        return history;
    }

    public async Task<ICollection<StarDustPointsHistory>> GetByUserIdAsync(int userId)
    {
        return await _context.StarDustPointsHistory
            .Where(h => h.UserId == userId)
            .Include(h => h.RelatedPost)
            .Include(h => h.RelatedComment)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task<ICollection<StarDustPointsHistory>> GetRecentActivityAsync(int userId, int days)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return await _context.StarDustPointsHistory
            .Where(h => h.UserId == userId && h.CreatedAt >= cutoffDate)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task<ICollection<StarDustPointsHistory>> GetTopEarnersAsync(int limit = 10)
    {
        return await _context.StarDustPointsHistory
            .GroupBy(h => h.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalPoints = g.Sum(h => h.Points),
                LatestActivity = g.Max(h => h.CreatedAt)
            })
            .OrderByDescending(x => x.TotalPoints)
            .Take(limit)
            .Join(_context.StarDustPointsHistory.Include(h => h.User),
                  x => x.UserId,
                  h => h.UserId,
                  (x, h) => h)
            .Distinct()
            .ToListAsync();
    }

    public async Task<int> GetTotalPointsEarnedAsync(int userId)
    {
        return await _context.StarDustPointsHistory
            .Where(h => h.UserId == userId)
            .SumAsync(h => h.Points);
    }
}