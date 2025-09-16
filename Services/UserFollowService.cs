using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;

namespace CodeQuestBackend.Services;

public class UserFollowService
{
    private readonly IUserFollowRepository _userFollowRepository;

    public UserFollowService(IUserFollowRepository userFollowRepository)
    {
        _userFollowRepository = userFollowRepository;
    }

    public async Task<bool> FollowSubcategoryAsync(int userId, int subcategoryId)
    {
        return await _userFollowRepository.FollowSubcategoryAsync(userId, subcategoryId);
    }

    public async Task<bool> UnfollowSubcategoryAsync(int userId, int subcategoryId)
    {
        return await _userFollowRepository.UnfollowSubcategoryAsync(userId, subcategoryId);
    }

    public async Task<bool> FollowAllSubcategoriesAsync(int userId, int categoryId)
    {
        return await _userFollowRepository.FollowAllSubcategoriesAsync(userId, categoryId);
    }

    public async Task<bool> IsFollowingSubcategoryAsync(int userId, int subcategoryId)
    {
        return await _userFollowRepository.IsFollowingSubcategoryAsync(userId, subcategoryId);
    }

    public async Task<UserFollowsDto> GetUserFollowsAsync(int userId)
    {
        return await _userFollowRepository.GetUserFollowsAsync(userId);
    }

    public async Task<List<SubcategoryDto>> GetFollowedSubcategoriesAsync(int userId)
    {
        return await _userFollowRepository.GetFollowedSubcategoriesAsync(userId);
    }

    public async Task<List<SubcategoryWithFollowerCountDto>> GetSubcategoriesWithFollowerCountAsync(int? userId = null)
    {
        return await _userFollowRepository.GetSubcategoriesWithFollowerCountAsync(userId);
    }

    public async Task<SubcategoryWithFollowerCountDto> GetSubcategoryWithFollowerCountAsync(int subcategoryId, int? userId = null)
    {
        return await _userFollowRepository.GetSubcategoryWithFollowerCountAsync(subcategoryId, userId);
    }

    public async Task<int> GetSubcategoryFollowerCountAsync(int subcategoryId)
    {
        return await _userFollowRepository.GetSubcategoryFollowerCountAsync(subcategoryId);
    }
}
