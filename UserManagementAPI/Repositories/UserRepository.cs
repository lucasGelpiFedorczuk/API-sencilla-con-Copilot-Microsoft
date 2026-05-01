using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _db;

    public UserRepository(UserDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return;
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }

    public async Task<User?> GetAsync(Guid id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }
}