using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models;

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

            var user = new User
            {
                Username = req.Username,
                PasswordHash = PasswordHasher.HashPassword(req.Password),

                FullName = req.FullName,
                FullNameKh = req.FullNameKh,  

                RoleId = req.RoleId,  
                outLetId = req.OutletId,

                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var newId = await _repo.CreateAsync(user);

            return newId > 0
                ? (true, "User created")
                : (false, "Create failed");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int id, UpdateUserRequest req)
        {
            var user = await _repo.GetByIdAsync(id);

            if (user == null)
                return (false, "User not found");

            user.FullName = req.FullName;
            user.FullNameKh = req.FullNameKh;   

            user.RoleId = req.RoleId;          
            user.IsActive = req.IsActive;

            var ok = await _repo.UpdateAsync(user);

            return ok
                ? (true, "User updated")
                : (false, "Update failed");
        }

      
        public Task<bool> DeleteAsync(int id)
            => _repo.DeleteAsync(id);
    }
}
