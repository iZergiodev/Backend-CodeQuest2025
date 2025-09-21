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
using CodeQuestBackend.Data;
using Microsoft.Extensions.Configuration;

namespace CodeQuestBackend.Repository;

public class UserRepository : IUserRepository
{

    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;
    private string? secretKey;

    public UserRepository(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
        secretKey = configuration.GetValue<string>("Jwt:Key");
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

    public async Task<User> UpdateDiscordFieldsAsync(User user)
    {
        // Update only Discord-related fields to preserve existing profile data
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existingUser != null)
        {
            existingUser.DiscordAccessToken = user.DiscordAccessToken;
            existingUser.DiscordRefreshToken = user.DiscordRefreshToken;
            existingUser.DiscordTokenExpiresAt = user.DiscordTokenExpiresAt;
            existingUser.DiscordUsername = user.DiscordUsername;
            existingUser.DiscordDiscriminator = user.DiscordDiscriminator;
            existingUser.DiscordAvatar = user.DiscordAvatar;
            
            await _db.SaveChangesAsync();
            return existingUser;
        }
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
        var jwtSettings = _configuration.GetSection("Jwt");
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? user.Username ?? ""),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("discord_id", user.DiscordId ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim("created_at", user.CreatedAt.ToString())
            }),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = handlerToken.CreateToken(tokenDescriptor);
        return new UserLoginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = new UserRegisterDto()
            {
                ID = user.Id.ToString(),
                Email = user.Email,
                Username = user.Username ?? $"Tripulante-{Random.Shared.Next(100000, 999999)}",
                Role = user.Role,
                Password = user.Password ?? "",
                Avatar = user.Avatar,
                Biography = user.Biography,
                BirthDate = user.BirthDate,
                CreatedAt = user.CreatedAt,
                StarDustPoints = user.StarDustPoints
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
            Name = createUserDto.Username, // Set name to username for email users
            Email = createUserDto.Email ?? "Email undefined",
            Role = createUserDto.Role,
            Password = encryptedPassword,
            CreatedAt = DateTime.UtcNow,
            BirthDate = DateTime.UtcNow.AddYears(-18), // Default to 18 years old
            StarDustPoints = 0
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _db.Users.AnyAsync(u => u.Id == id);
    }

    public async Task<ICollection<User>> GetUsersByIdsAsync(ICollection<int> userIds)
    {
        return await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

}
