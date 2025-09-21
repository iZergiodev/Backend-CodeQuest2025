using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Services;
using CodeQuestBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeQuestBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly TrendingService _trendingService;

        public PostsController(PostService postService, TrendingService trendingService)
        {
            _postService = postService;
            _trendingService = trendingService;
        }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        try
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts: {ex.Message}");
        }
    }

    [HttpGet("{id:int}", Name = "GetPost")]
    public async Task<IActionResult> GetPostById(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var post = await _postService.GetPostByIdAsync(id, currentUserId);
            if (post == null)
            {
                return NotFound($"El post con ID {id} no existe");
            }

            // Record view engagement for trending (only if user is authenticated)
            if (currentUserId.HasValue)
            {
                await _trendingService.RecordEngagementAsync(id, currentUserId.Value, EngagementType.View);
            }

            return Ok(post);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener el post: {ex.Message}");
        }
    }

    [HttpGet("author/{authorId:int}")]
    public async Task<IActionResult> GetPostsByAuthor(int authorId)
    {
        try
        {
            var posts = await _postService.GetPostsByAuthorIdAsync(authorId);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts del autor: {ex.Message}");
        }
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetPostsByCategory(int categoryId)
    {
        try
        {
            var posts = await _postService.GetPostsByCategoryIdAsync(categoryId);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts de la categoría: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto, [FromQuery] int authorId)
    {
        try
        {
            if (createPostDto == null)
            {
                return BadRequest("El cuerpo de la solicitud no puede estar vacío.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await _postService.CreatePostAsync(createPostDto, authorId);
            return CreatedAtRoute("GetPost", new { id = post.Id }, post);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al crear el post: {ex.Message}");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] CreatePostDto updatePostDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await _postService.UpdatePostAsync(id, updatePostDto);
            if (post == null)
            {
                return NotFound($"El post con ID {id} no existe");
            }

            return Ok(post);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al actualizar el post: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            if (!await _postService.PostExistsAsync(id))
            {
                return NotFound($"El post con ID {id} no existe");
            }

            var deleted = await _postService.DeletePostAsync(id);
            if (!deleted)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar el post");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar el post: {ex.Message}");
        }
    }

    [HttpPost("{id:int}/visit")]
    public async Task<IActionResult> IncrementVisits(int id)
    {
        try
        {
            var success = await _postService.IncrementVisitsAsync(id);
            if (!success)
            {
                return NotFound($"El post con ID {id} no existe");
            }

            return Ok(new { message = "Visita registrada correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al registrar la visita: {ex.Message}");
        }
    }

    [HttpGet("ranked")]
    public async Task<IActionResult> GetRankedPosts()
    {
        try
        {
            var posts = await _postService.GetRankedPostsAsync();
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts rankeados: {ex.Message}");
        }
    }

    [HttpGet("ranked/category/{categoryId:int}")]
    public async Task<IActionResult> GetRankedPostsByCategory(int categoryId)
    {
        try
        {
            var posts = await _postService.GetRankedPostsByCategoryAsync(categoryId);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts rankeados por categoría: {ex.Message}");
        }
    }

    // Paginated endpoints
    [HttpGet("paginated")]
    public async Task<IActionResult> GetAllPostsPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var posts = await _postService.GetAllPostsPaginatedAsync(page, pageSize);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts paginados: {ex.Message}");
        }
    }

    [HttpGet("author/{authorId:int}/paginated")]
    public async Task<IActionResult> GetPostsByAuthorPaginated(int authorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var posts = await _postService.GetPostsByAuthorIdPaginatedAsync(authorId, page, pageSize);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts del autor paginados: {ex.Message}");
        }
    }

    [HttpGet("category/{categoryId:int}/paginated")]
    public async Task<IActionResult> GetPostsByCategoryPaginated(int categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var posts = await _postService.GetPostsByCategoryIdPaginatedAsync(categoryId, page, pageSize);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts de la categoría paginados: {ex.Message}");
        }
    }

    [HttpGet("subcategory/{subcategoryId:int}/paginated")]
    public async Task<IActionResult> GetPostsBySubcategoryPaginated(int subcategoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var posts = await _postService.GetPostsBySubcategoryIdPaginatedAsync(subcategoryId, page, pageSize);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts de la subcategoría paginados: {ex.Message}");
        }
    }

    [HttpGet("ranked/paginated")]
    public async Task<IActionResult> GetRankedPostsPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var posts = await _postService.GetRankedPostsPaginatedAsync(page, pageSize);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts rankeados paginados: {ex.Message}");
        }
    }

    [HttpGet("ranked/category/{categoryId:int}/paginated")]
    public async Task<IActionResult> GetRankedPostsByCategoryPaginated(int categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var posts = await _postService.GetRankedPostsByCategoryPaginatedAsync(categoryId, page, pageSize);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts rankeados por categoría paginados: {ex.Message}");
        }
    }

    [HttpGet("followed/paginated")]
    public async Task<IActionResult> GetPostsByFollowedSubcategoriesPaginated(
        [FromQuery] string subcategoryIds, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "recent")
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            // Parse comma-separated subcategory IDs
            var subcategoryIdList = new List<int>();
            if (!string.IsNullOrEmpty(subcategoryIds))
            {
                subcategoryIdList = subcategoryIds.Split(',')
                    .Where(id => int.TryParse(id.Trim(), out _))
                    .Select(id => int.Parse(id.Trim()))
                    .ToList();
            }

            var posts = await _postService.GetPostsByFollowedSubcategoriesAsync(subcategoryIdList, page, pageSize, sortBy);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los posts de subcategorías seguidas: {ex.Message}");
        }
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }
        return null;
    }
}