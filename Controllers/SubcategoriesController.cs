using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeQuestBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubcategoriesController : ControllerBase
{
    private readonly ISubcategoryRepository _subcategoryRepository;
    private readonly ICategoryRepository _categoryRepository;

    public SubcategoriesController(ISubcategoryRepository subcategoryRepository, ICategoryRepository categoryRepository)
    {
        _subcategoryRepository = subcategoryRepository;
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSubcategories()
    {
        try
        {
            var subcategories = await _subcategoryRepository.GetAllAsync();
            var subcategoryDtos = subcategories.Select(MapToSubcategoryDto).ToList();
            return Ok(subcategoryDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las subcategorías: {ex.Message}");
        }
    }

    [HttpGet("{id:int}", Name = "GetSubcategory")]
    public async Task<IActionResult> GetSubcategoryById(int id)
    {
        try
        {
            var subcategory = await _subcategoryRepository.GetByIdAsync(id);
            if (subcategory == null)
            {
                return NotFound($"La subcategoría con ID {id} no existe");
            }

            var subcategoryDto = MapToSubcategoryDto(subcategory);
            return Ok(subcategoryDto);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener la subcategoría: {ex.Message}");
        }
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetSubcategoriesByCategory(int categoryId)
    {
        try
        {
            var subcategories = await _subcategoryRepository.GetByCategoryIdAsync(categoryId);
            var subcategoryDtos = subcategories.Select(MapToSubcategoryDto).ToList();
            return Ok(subcategoryDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las subcategorías de la categoría: {ex.Message}");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateSubcategory([FromBody] CreateSubcategoryDto createSubcategoryDto)
    {
        try
        {
            if (createSubcategoryDto == null)
            {
                return BadRequest("El cuerpo de la solicitud no puede estar vacío.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar que la categoría padre existe
            if (!await _categoryRepository.ExistsAsync(createSubcategoryDto.CategoryId))
            {
                return BadRequest($"La categoría con ID {createSubcategoryDto.CategoryId} no existe.");
            }

            // Verificar que no existe una subcategoría con el mismo nombre
            if (await _subcategoryRepository.ExistsByNameAsync(createSubcategoryDto.Name))
            {
                return BadRequest($"Ya existe una subcategoría with el nombre '{createSubcategoryDto.Name}'.");
            }

            var subcategory = await _subcategoryRepository.CreateAsync(createSubcategoryDto);
            var subcategoryDto = MapToSubcategoryDto(subcategory);

            return CreatedAtRoute("GetSubcategory", new { id = subcategory.Id }, subcategoryDto);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al crear la subcategoría: {ex.Message}");
        }
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateSubcategory(int id, [FromBody] UpdateSubcategoryDto updateSubcategoryDto)
    {
        try
        {
            if (updateSubcategoryDto == null)
            {
                return BadRequest("El cuerpo de la solicitud no puede estar vacío.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _subcategoryRepository.ExistsAsync(id))
            {
                return NotFound($"La subcategoría con ID {id} no existe");
            }

            // Verificar que la categoría padre existe
            if (!await _categoryRepository.ExistsAsync(updateSubcategoryDto.CategoryId))
            {
                return BadRequest($"La categoría con ID {updateSubcategoryDto.CategoryId} no existe.");
            }

            var subcategory = await _subcategoryRepository.UpdateAsync(id, updateSubcategoryDto);
            if (subcategory == null)
            {
                return NotFound($"La subcategoría con ID {id} no existe");
            }

            var subcategoryDto = MapToSubcategoryDto(subcategory);
            return Ok(subcategoryDto);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al actualizar la subcategoría: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteSubcategory(int id)
    {
        try
        {
            if (!await _subcategoryRepository.ExistsAsync(id))
            {
                return NotFound($"La subcategoría con ID {id} no existe");
            }

            var deleted = await _subcategoryRepository.DeleteAsync(id);
            if (!deleted)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar la subcategoría");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar la subcategoría: {ex.Message}");
        }
    }

    private static SubcategoryDto MapToSubcategoryDto(CodeQuestBackend.Models.Subcategory subcategory)
    {
        return new SubcategoryDto
        {
            Id = subcategory.Id,
            Name = subcategory.Name,
            Description = subcategory.Description,
            Color = subcategory.Color,
            CreatedAt = subcategory.CreatedAt,
            UpdatedAt = subcategory.UpdatedAt,
            CategoryId = subcategory.CategoryId,
            CategoryName = subcategory.Category?.Name ?? string.Empty
        };
    }
}