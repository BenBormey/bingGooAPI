using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models;
using bingGooAPI.Models.User;
using Microsoft.AspNetCore.Identity;

namespace bingGooAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<User>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<User?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public async Task<(bool Success, string Message)> CreateAsync(CreateUserRequest req)
        {
           
            var exist = await _repo.GetByUsernameAsync(req.Username);

            if (exist != null)
                return (false, "Username already exists");

            // Map DTO → Entity
            var user = new User
            {
                Username = req.Username,
                PasswordHash = PasswordHasher.HashPassword(req.Password),

                FullName = req.FullName,
                FullNameKh = req.FullNameKh,

                RoleId = req.RoleId,

                Phone = req.Phone,
                Email = req.Email,

                address = req.Address,
                addressKh = req.AddressKh,

                outLetId = req.OutletId,

                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var newId = await _repo.CreateAsync(user);

            return newId > 0
                ? (true, "User created successfully")
                : (false, "Create failed");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int id, UpdateUserDto req)
        {
            var user = await _repo.GetByIdAsync(id);

            if (user == null)
                return (false, "User not found");

            // Map Request → DTO
            var dto = new UpdateUserDto
            {
                Id = id,

                Username = user.Username, // Keep old username

                FullName = req.FullName,
                FullNameKh = req.FullNameKh,

                RoleId = req.RoleId,

                Phone = req.Phone,
                Email = req.Email,

                Address = req.Address,
                AddressKh = req.AddressKh,

                IsActive = req.IsActive,

                OutletId = req.OutletId
            };

            var ok = await _repo.UpdateAsync(dto);

            return ok
                ? (true, "User updated successfully")
                : (false, "Update failed");
        }



        public Task<bool> DeleteAsync(int id)
            => _repo.DeleteAsync(id);

        public async Task<(bool Success, string Message)> ChangePasswordAsync(
      int userId,
      string oldPassword,
      string newPassword)
        {
            var user = await _repo.GetByIdAsync(userId);

            if (user == null)
                return (false, "User not found");

            // ✅ Verify old password
            if (!PasswordHasher.VerifyPassword(user.PasswordHash, oldPassword))
                return (false, "Old password incorrect");

            // ✅ Hash new password
            var newHash = PasswordHasher.HashPassword(newPassword);

            var ok = await _repo.ChangePasswordAsync(userId, newHash);

            if (!ok)
                return (false, "Update failed");

            return (true, "Password changed successfully");
        }


        public async Task<(bool Success, string Message)> ResetPasswordAsync(
       int userId,
       string newPassword)
        {
            var user = await _repo.GetByIdAsync(userId);

            if (user == null)
                return (false, "User not found");

            var hash = PasswordHasher.HashPassword(newPassword);

            var ok = await _repo.ResetPasswordAsync(userId, hash);

            if (!ok)
                return (false, "Reset failed");

            return (true, "Password reset done");
        }


    }
}
