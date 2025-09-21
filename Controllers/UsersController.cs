using AutoMapper;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeQuestBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetAllUsers();
            var usersDto = _mapper.Map<List<UserDto>>(users);
            return Ok(usersDto);
        }

        [HttpGet("{id:int}", Name = "GetUser")]
        public IActionResult GetUser(int id)
        {
            var user = _userRepository.GetUser(id);
            if (user == null)
            {
                return NotFound($"El usuario con el id {id} no existe");
            }
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);

        }

        [HttpPost("Register", Name = "RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(createUserDto.Email))
            {
                return BadRequest("Email es requerido");
            }
            if (!_userRepository.IsUniqueEmail(createUserDto.Email))
            {
                return BadRequest("El usuario ya existe");
            }
            var result = await _userRepository.Register(createUserDto);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al registrar el usuario");
            }
            return CreatedAtRoute("GetUser", new { id = result.Id }, result);
        }

        [HttpPost("Login", Name = "LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto)
        {
            if (userLoginDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userRepository.Login(userLoginDto);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                // Get current user ID from token
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out int currentUserId))
                {
                    return Unauthorized(new { message = "Invalid token claims" });
                }

                // Users can only update their own profile
                if (currentUserId != id)
                {
                    return Forbid("You can only update your own profile");
                }

                if (updateUserDto == null || !ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = _userRepository.GetUser(id);
                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                // Update user properties
                if (!string.IsNullOrEmpty(updateUserDto.Name))
                    existingUser.Name = updateUserDto.Name;
                
                if (!string.IsNullOrEmpty(updateUserDto.Biography))
                    existingUser.Biography = updateUserDto.Biography;
                
                if (!string.IsNullOrEmpty(updateUserDto.BirthDate))
                {
                    if (DateTime.TryParse(updateUserDto.BirthDate, out DateTime birthDate))
                        existingUser.BirthDate = DateTime.SpecifyKind(birthDate, DateTimeKind.Utc);
                }
                
                if (!string.IsNullOrEmpty(updateUserDto.Avatar))
                    existingUser.Avatar = updateUserDto.Avatar;

                var updatedUser = await _userRepository.UpdateAsync(existingUser);
                var userDto = _mapper.Map<UserDto>(updatedUser);

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error updating user", error = ex.Message });
            }
        }

    }
}
