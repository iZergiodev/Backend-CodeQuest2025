using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using CodeQuestBackend.Data;

namespace CodeQuestBackend.Repository;

public class SubcategoryRepository : ISubcategoryRepository
{
    private readonly ApplicationDbContext _context;

    public SubcategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Subcategory>> GetAllAsync()
    {
        return await _context.Subcategories
            .Include(s => s.Category)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Subcategory?> GetByIdAsync(int id)
    {
        return await _context.Subcategories
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<ICollection<Subcategory>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.Subcategories
            .Include(s => s.Category)
            .Where(s => s.CategoryId == categoryId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Subcategory?> GetByNameAsync(string name)
    {
        return await _context.Subcategories
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
    }

    public async Task<Subcategory> CreateAsync(CreateSubcategoryDto createSubcategoryDto)
    {
        var subcategory = new Subcategory
        {
            Name = createSubcategoryDto.Name,
            Description = createSubcategoryDto.Description,
            Color = createSubcategoryDto.Color,
            CategoryId = createSubcategoryDto.CategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Subcategories.Add(subcategory);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(subcategory.Id) ?? subcategory;
    }

    public async Task<Subcategory?> UpdateAsync(int id, UpdateSubcategoryDto updateSubcategoryDto)
    {
        var subcategory = await _context.Subcategories
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subcategory == null)
            return null;

        subcategory.Name = updateSubcategoryDto.Name;
        subcategory.Description = updateSubcategoryDto.Description;
        subcategory.Color = updateSubcategoryDto.Color;
        subcategory.CategoryId = updateSubcategoryDto.CategoryId;
        subcategory.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(subcategory.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subcategory = await _context.Subcategories
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subcategory == null)
            return false;

        _context.Subcategories.Remove(subcategory);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Subcategories
            .AnyAsync(s => s.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Subcategories
            .AnyAsync(s => s.Name.ToLower() == name.ToLower());
    }
}