using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeQuestBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoriesController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las categorías: {ex.Message}");
        }
    }

    [HttpGet("{id:int}", Name = "GetCategory")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound($"La categoría con ID {id} no existe");
            }
            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener la categoría: {ex.Message}");
        }
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetCategoryByName(string name)
    {
        try
        {
            var category = await _categoryService.GetCategoryByNameAsync(name);
            if (category == null)
            {
                return NotFound($"La categoría '{name}' no existe");
            }
            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener la categoría: {ex.Message}");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al crear la categoría: {ex.Message}");
        }
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
            if (category == null)
            {
                return NotFound($"La categoría con ID {id} no existe");
            }

            return Ok(category);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al actualizar la categoría: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            if (!await _categoryService.CategoryExistsAsync(id))
            {
                return NotFound($"La categoría con ID {id} no existe");
            }

            var deleted = await _categoryService.DeleteCategoryAsync(id);
            if (!deleted)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar la categoría");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar la categoría: {ex.Message}");
        }
    }
}