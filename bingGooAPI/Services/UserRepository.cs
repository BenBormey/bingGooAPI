using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.User;
using Dapper;
using Microsoft.EntityFrameworkCore;
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

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var sql = @"
SELECT 
    u.Id,
    u.Username,
    u.PasswordHash,
    u.FullName,
    u.FullNameKh,
    u.RoleId,
    r.RoleName,
    u.Phone,
    u.email AS Email,
    u.address AS Address,
    u.addressKh AS AddressKh,
    u.IsActive,
    u.CreatedAt,
    u.LastLoginAt,
    o.OutletName,
    o.Id AS OutletId
FROM Users u
JOIN Roles r ON u.RoleId = r.Id
JOIN Outlet o ON o.Id = u.OutletId
WHERE u.Username = @Username
and u.IsActive =1
";

            return await _db.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Username = username }
            );
        }

      
        public async Task<User?> GetByIdAsync(int id)
        {
            var sql = @"
SELECT 
    u.*,
    r.RoleName,
    o.OutletName
FROM Users u
JOIN Roles r ON u.RoleId = r.Id
JOIN Outlet o ON o.Id = u.OutletId
WHERE u.Id = @Id and  u.IsActive =1
";

            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(User user)
        {
            var sql = @"
INSERT INTO Users
(
    Username,
    PasswordHash,
    FullName,
    FullNameKh,
    RoleId,
    Phone,
    email,
    address,
    addressKh,
    IsActive,
    CreatedAt,
    OutletId
)
VALUES
(
    @Username,
    @PasswordHash,
    @FullName,
    @FullNameKh,
    @RoleId,
    @Phone,
    @Email,
    @Address,
    @AddressKh,
    @IsActive,
    @CreatedAt,
    @OutletId
);

SELECT CAST(SCOPE_IDENTITY() AS INT);
";

            return await _db.ExecuteScalarAsync<int>(sql, user);
        }


        public async Task<bool> UpdateLastLoginAsync(int id)
        {
            var rows = await _db.ExecuteAsync(
                "UPDATE Users SET LastLoginAt = GETUTCDATE() WHERE Id = @Id",
                new { Id = id }
            );

            return rows > 0;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var sql = @"
SELECT 
    u.*,
    r.RoleName,
    o.OutletName
FROM Users u
JOIN Roles r ON u.RoleId = r.Id
JOIN Outlet o ON o.Id = u.OutletId

where u.IsActive =1
";

            return await _db.QueryAsync<User>(sql);
        }


        public async Task<bool> UpdateAsync(UpdateUserDto user)
        {
            var sql = @"
UPDATE Users
SET 
    Username   = @Username,
    FullName   = @FullName,
    FullNameKh = @FullNameKh,
    RoleId     = @RoleId,
    Phone      = @Phone,
    email      = @Email,
    address    = @Address,
    addressKh  = @AddressKh,
    IsActive   = @IsActive,
    OutletId   = @OutletId
WHERE Id = @Id
";

            var rows = await _db.ExecuteAsync(sql, user);

            return rows > 0;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _db.ExecuteAsync(
                "UPDATE Users SET IsActive = 0 WHERE Id = @Id",
                new { Id = id }
            );

            return rows > 0;
        }

        public async Task<bool> ChangePasswordAsync(int id, string passwordHash)
        {
            var sql = @"UPDATE Users 
                SET PasswordHash = @PasswordHash 
                WHERE Id = @Id";

            var result = await _db.ExecuteAsync(sql, new
            {
                Id = id,
                PasswordHash = passwordHash
            });

            return result > 0;
        }



        public async Task<bool> ResetPasswordAsync(int id, string passwordHash)
        {
            var sql = @"UPDATE Users 
                SET PasswordHash = @PasswordHash 
                WHERE Id = @Id";

            var result = await _db.ExecuteAsync(sql, new
            {
                Id = id,
                PasswordHash = passwordHash
            });

            return result > 0;
        }

    }
}
