using System;
using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;

namespace CodeQuestBackend.Repository.IRepository;

public interface IUserRepository
{
    ICollection<User> GetAllUsers();
    User? GetUser(int id);
    bool IsUniqueEmail(string email);
    Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
    Task<User> Register(CreateUserDto createUserDto);
    Task<User?> GetByDiscordIdAsync(string discordId);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> ExistsAsync(int id);
}