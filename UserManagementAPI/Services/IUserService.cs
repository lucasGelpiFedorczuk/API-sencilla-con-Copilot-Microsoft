using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public interface IUserService
{
    Task<User?> GetAsync(Guid id);
    Task<List<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null);
}