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
    u.IsActive,
    u.CreatedAt,
    u.LastLoginAt,
	o.OutletName
	,o.Id as outLetId
FROM Users u
JOIN Roles r ON u.RoleId = r.Id
join Outlet o 
on o.Id = u.[OutletId]
WHERE u.Username = @Username
";

            return await _db.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Username = username }
            );
        }




        public Task<User?> GetByIdAsync(int id) =>
            _db.QueryFirstOrDefaultAsync<User>(@"
SELECT  u.*,
        r.Id as RoleId,
        r.RoleName,
		o.Id as outletId,
		o.OutletName
FROM Users u
JOIN Roles r ON u.RoleId = r.Id
join Outlet o 
on o.Id = u.[OutletId]
WHERE u.Id = @Id
",
            new { Id = id });

        public Task<int> CreateAsync(User user)
        {
            var sql = @"
INSERT INTO Users 
(
    Username,
    PasswordHash,
    FullName,
    FullNameKh,
    RoleId,
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
    @IsActive,
    @CreatedAt,
    @outLetId
);

SELECT CAST(SCOPE_IDENTITY() AS int);
";

            return _db.ExecuteScalarAsync<int>(sql, user);
        }

        public Task<bool> UpdateLastLoginAsync(int id) =>
            _db.ExecuteAsync(
                "UPDATE Users SET LastLoginAt = GETUTCDATE() WHERE Id = @Id",
                new { Id = id }
            ).ContinueWith(t => t.Result > 0);

     
        public Task<IEnumerable<User>> GetAllAsync() =>
            _db.QueryAsync<User>(@"
SELECT  u.*,
        r.Id as RoleId,
        r.RoleName,
		o.Id as outletId,
		o.OutletName
FROM Users u
JOIN Roles r ON u.RoleId = r.Id
join Outlet o 
on o.Id = u.[OutletId]
");

     
        public Task<bool> UpdateAsync(User user) =>
            _db.ExecuteAsync(@"
UPDATE Users
SET 
    Username @Username,
    FullName   = @FullName,
    FullNameKh = @FullNameKh,
    RoleId     = @RoleId,
    IsActive   = @IsActive,
    OutLetId   = @OutLetId
WHERE Id = @Id
", user).ContinueWith(t => t.Result > 0);

        
        public Task<bool> DeleteAsync(int id) =>
            _db.ExecuteAsync(
                "DELETE FROM Users WHERE Id = @Id",
                new { Id = id }
            ).ContinueWith(t => t.Result > 0);
    }
}
