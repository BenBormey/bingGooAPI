using bingGooAPI.Entities;
using bingGooAPI.Models;
using bingGooAPI.Models.User;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<(bool Success, string Message)> CreateAsync(CreateUserRequest req);
    Task<(bool Success, string Message)> UpdateAsync(int id, UpdateUserDto req);
    Task<bool> DeleteAsync(int id);


    Task<(bool Success, string Message)> ChangePasswordAsync(
        int userId,
        string oldPassword,
        string newPassword);

    Task<(bool Success, string Message)> ResetPasswordAsync(
        int userId,
        string newPassword);
}
