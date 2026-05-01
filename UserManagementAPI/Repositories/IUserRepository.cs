using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid id);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null);
}