using System;
using CodeQuestBackend.Models;
using CodeQuestBackend.Models.Dtos;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Repository;

public class UserRepository : IUserRepository
{

    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public User? GetUser(int id)
    {
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<User> GetUsers()
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

}
