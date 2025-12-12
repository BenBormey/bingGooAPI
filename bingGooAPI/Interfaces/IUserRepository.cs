using bingGooAPI.Entities;

namespace bingGooAPI.Interfaces
{
    public interface IUserRepository
    {
        // 🔐 Auth
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task<int> CreateAsync(User user);
        Task<bool> UpdateLastLoginAsync(int id);

        // 🧾 CRUD
        Task<IEnumerable<User>> GetAllAsync();
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
    }
}
