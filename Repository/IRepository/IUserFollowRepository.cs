using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;

namespace CodeQuestBackend.Repository.IRepository;

public interface IUserFollowRepository
{
    Task<bool> FollowSubcategoryAsync(int userId, int subcategoryId);
    Task<bool> UnfollowSubcategoryAsync(int userId, int subcategoryId);
    Task<bool> FollowAllSubcategoriesAsync(int userId, int categoryId);
    Task<bool> IsFollowingSubcategoryAsync(int userId, int subcategoryId);
    Task<UserFollowsDto> GetUserFollowsAsync(int userId);
    Task<List<SubcategoryDto>> GetFollowedSubcategoriesAsync(int userId);
    Task<List<SubcategoryWithFollowerCountDto>> GetSubcategoriesWithFollowerCountAsync(int? userId = null);
    Task<SubcategoryWithFollowerCountDto> GetSubcategoryWithFollowerCountAsync(int subcategoryId, int? userId = null);
    Task<int> GetSubcategoryFollowerCountAsync(int subcategoryId);
    Task<ICollection<UserSubcategoryFollow>> GetFollowersBySubcategoryIdAsync(int subcategoryId);
}
