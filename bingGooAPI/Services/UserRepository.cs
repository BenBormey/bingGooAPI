using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository(IDbConnection db)
        {
            _db = db;
        }

        // 🔐 Auth
        public Task<User?> GetByUsernameAsync(string username) =>
            _db.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Username=@Username",
                new { Username = username }
            );

        public Task<User?> GetByIdAsync(int id) =>
            _db.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Id=@Id",
                new { Id = id }
            );

        public Task<int> CreateAsync(User user)
        {
            var sql = @"
INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive)
VALUES (@Username, @PasswordHash, @FullName, @Role, @IsActive);
SELECT CAST(SCOPE_IDENTITY() as int);";
            return _db.ExecuteScalarAsync<int>(sql, user);
        }

        public Task<bool> UpdateLastLoginAsync(int id) =>
            _db.ExecuteAsync(
                "UPDATE Users SET LastLoginAt=GETUTCDATE() WHERE Id=@Id",
                new { Id = id }
            ).ContinueWith(t => t.Result > 0);

        // 🧾 CRUD
        public Task<IEnumerable<User>> GetAllAsync() =>
            _db.QueryAsync<User>(
                "SELECT Id, Username, FullName, Role, IsActive FROM Users"
            );

        public Task<bool> UpdateAsync(User user) =>
            _db.ExecuteAsync(@"
UPDATE Users
SET FullName=@FullName, Role=@Role, IsActive=@IsActive
WHERE Id=@Id", user).ContinueWith(t => t.Result > 0);

        public Task<bool> DeleteAsync(int id) =>
            _db.ExecuteAsync(
                "DELETE FROM Users WHERE Id=@Id",
                new { Id = id }
            ).ContinueWith(t => t.Result > 0);
    }
}
