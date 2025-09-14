using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeQuestBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly PostService _postService;

    public PostsController(PostService postService)
    {
        _postService = postService;
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
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound($"El post con ID {id} no existe");
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
}