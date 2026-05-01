using UserManagementAPI.Models;
using UserManagementAPI.Repositories;

namespace UserManagementAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<User> CreateAsync(User user)
    {
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        await _repo.AddAsync(user);
        return user;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repo.GetAsync(id);
        if (existing is null) return false;
        await _repo.DeleteAsync(id);
        return true;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<User?> GetAsync(Guid id)
    {
        return await _repo.GetAsync(id);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var existing = await _repo.GetAsync(user.Id);
        if (existing is null) return false;
        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Email = user.Email;
        await _repo.UpdateAsync(existing);
        return true;
    }
}