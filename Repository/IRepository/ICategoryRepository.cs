using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;

namespace CodeQuestBackend.Repository.IRepository;

public interface ICategoryRepository
{
    Task<ICollection<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetByNameAsync(string name);
    Task<Category> CreateAsync(CreateCategoryDto createCategoryDto);
    Task<Category?> UpdateAsync(int id, UpdateCategoryDto updateCategoryDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
}