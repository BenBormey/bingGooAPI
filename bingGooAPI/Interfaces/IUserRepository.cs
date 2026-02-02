using bingGooAPI.Entities;
using bingGooAPI.Models.User;

public interface IUserRepository
{
    // 🔐 Auth
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(int id);
    Task<int> CreateAsync(User user);
    Task<bool> UpdateLastLoginAsync(int id);

    // 🔐 Password
    Task<bool> ChangePasswordAsync(int id, string newPasswordHash);
    Task<bool> ResetPasswordAsync(int id, string newPasswordHash);

    // 🧾 CRUD
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> UpdateAsync(UpdateUserDto user);
    Task<bool> DeleteAsync(int id);
}
