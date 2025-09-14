using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;

namespace CodeQuestBackend.Repository.IRepository;

public interface ISubcategoryRepository
{
    Task<ICollection<Subcategory>> GetAllAsync();
    Task<Subcategory?> GetByIdAsync(int id);
    Task<ICollection<Subcategory>> GetByCategoryIdAsync(int categoryId);
    Task<Subcategory?> GetByNameAsync(string name);
    Task<Subcategory> CreateAsync(CreateSubcategoryDto createSubcategoryDto);
    Task<Subcategory?> UpdateAsync(int id, UpdateSubcategoryDto updateSubcategoryDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
}