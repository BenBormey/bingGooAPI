using bingGooAPI.Entities;
using bingGooAPI.Models;

namespace bingGooAPI.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<(bool Success, string Message)> CreateAsync(CreateUserRequest req);
        Task<(bool Success, string Message)> UpdateAsync(int id, UpdateUserRequest req);
        Task<bool> DeleteAsync(int id);
    }
}
