using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Repository;

public class UserRepository : IUserRepository
{

    private readonly ApplicationDbContext _db;
    private string? secretKey;

    public UserRepository(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    }

    public User? GetUser(int id)
    {
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<User> GetUsers()
    {
        return _db.Users.OrderBy(u => u.Username).ToList();
    }

    public ICollection<User> GetAllUsers()
    {
        return _db.Users.OrderBy(u => u.Username).ToList();
    }

    public bool IsUniqueEmail(string email)
    {
        return !_db.Users.Any(u => u.Email.ToLower().Trim() == email.ToLower().Trim());
    }


    public async Task<User?> GetByDiscordIdAsync(string discordId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.DiscordId == discordId);
    }

    public async Task<User> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return user;
    }
    
    public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.Email))
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "El email es requerido"
            };
        }
        var user = await _db.Users.FirstOrDefaultAsync<User>(u => u.Email.ToLower().Trim() == userLoginDto.Email.ToLower().Trim());
        if (user == null)
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Usuario no encontrado"
            };
        }
        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Credenciales incorrectas"
            };
        }
        //JWT
        var handlerToken = new JwtSecurityTokenHandler();
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("SecreyKey no está configurada");
        }
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id",user.Id.ToString()),
                new Claim("email",user.Email),
                new Claim(ClaimTypes.Role,user.Role ?? string.Empty),

            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = handlerToken.CreateToken(tokenDescriptor);
        return new UserLoginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = new UserRegisterDto()
            {
                Email = user.Email,
                Username = user.Username ?? $"Tripulante-{Random.Shared.Next(100000, 999999)}",
                Role = user.Role,
                Password = user.Password ?? ""
            },
            Message = "Usuario logueado correctamente"
        };
    }

    public async Task<User> Register(CreateUserDto createUserDto)
    {
        var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        var user = new User()
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email ?? "Email undefined",
            Role = createUserDto.Role,
            Password = encryptedPassword
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

}
