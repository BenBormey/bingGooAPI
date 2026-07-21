using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.User;
using JuJuBiAPI.Queries;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _db.QueryFirstOrDefaultAsync<User>(
                UserQueries.GetByUsername,
                new { Username = username }
            );
        }

        public async Task<IEnumerable<User>> GetByRoleCodeAsync(string roleCode)
        {
            return await _db.QueryAsync<User>(
                UserQueries.GetByRoleCode,
                new { RoleCode = roleCode }
            );
        }


        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<User>(UserQueries.GetById, new { Id = id });
        }

        public async Task<int> CreateAsync(User user)
        {
            return await _db.ExecuteScalarAsync<int>(UserQueries.Create, user);
        }


        public async Task<bool> UpdateLastLoginAsync(int id)
        {
            var rows = await _db.ExecuteAsync(
                UserQueries.UpdateLastLogin,
                new { Id = id }
            );

            return rows > 0;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.QueryAsync<User>(UserQueries.GetAll);
        }


        public async Task<bool> UpdateAsync(UpdateUserDto user)
        {
            var rows = await _db.ExecuteAsync(UserQueries.Update, user);

            return rows > 0;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _db.ExecuteAsync(
                UserQueries.Delete,
                new { Id = id }
            );

            return rows > 0;
        }

        public async Task<bool> ChangePasswordAsync(int id, string passwordHash)
        {
            var result = await _db.ExecuteAsync(UserQueries.ChangePassword, new
            {
                Id = id,
                PasswordHash = passwordHash
            });

            return result > 0;
        }



        public async Task<bool> ResetPasswordAsync(int id, string passwordHash)
        {
            var result = await _db.ExecuteAsync(UserQueries.ResetPassword, new
            {
                Id = id,
                PasswordHash = passwordHash
            });

            return result > 0;
        }

    }
}
