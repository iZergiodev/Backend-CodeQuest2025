using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;

namespace CodeQuestBackend.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ICollection<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToCategoryDto).ToList();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category != null ? MapToCategoryDto(category) : null;
    }

    public async Task<CategoryDto?> GetCategoryByNameAsync(string name)
    {
        var category = await _categoryRepository.GetByNameAsync(name);
        return category != null ? MapToCategoryDto(category) : null;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        if (await _categoryRepository.ExistsByNameAsync(createCategoryDto.Name))
        {
            throw new InvalidOperationException($"Category with name '{createCategoryDto.Name}' already exists.");
        }

        var category = await _categoryRepository.CreateAsync(createCategoryDto);
        return MapToCategoryDto(category);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
    {
        if (!await _categoryRepository.ExistsAsync(id))
        {
            return null;
        }

        if (await _categoryRepository.ExistsByNameAsync(updateCategoryDto.Name))
        {
            var existingCategory = await _categoryRepository.GetByNameAsync(updateCategoryDto.Name);
            if (existingCategory != null && existingCategory.Id != id)
            {
                throw new InvalidOperationException($"Category with name '{updateCategoryDto.Name}' already exists.");
            }
        }

        var updatedCategory = await _categoryRepository.UpdateAsync(id, updateCategoryDto);
        return updatedCategory != null ? MapToCategoryDto(updatedCategory) : null;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        return await _categoryRepository.DeleteAsync(id);
    }

    public async Task<bool> CategoryExistsAsync(int id)
    {
        return await _categoryRepository.ExistsAsync(id);
    }

    private static CategoryDto MapToCategoryDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}