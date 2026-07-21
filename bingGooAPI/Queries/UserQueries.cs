namespace JuJuBiAPI.Queries
{
    public static class UserQueries
    {
        public const string GetByUsername = @"
SELECT
    u.Id,
    u.Username,
    u.PasswordHash,
    u.FullName,
    u.FullNameKh,
    u.RoleId,
    r.RoleName,
    r.RoleCode,
    u.Phone,
    u.email AS Email,
    u.address AS Address,
    u.addressKh AS AddressKh,
    u.IsActive,
    u.HasSystemAccess,
    u.CreatedAt,
    u.LastLoginAt,
    o.OutletName,
    u.OutletId
FROM Users u
JOIN Roles r ON u.RoleId = r.Id
LEFT JOIN Outlet o ON o.Id = u.OutletId
WHERE u.Username = @Username
AND u.IsActive = 1
";

        public const string GetByRoleCode = @"
SELECT
    u.Id,
    u.Username,
    u.PasswordHash,
    u.FullName,
    u.FullNameKh,
    u.RoleId,
    r.RoleName,
    r.RoleCode,
    u.Phone,
    u.email AS Email,
    u.address AS Address,
    u.addressKh AS AddressKh,
    u.IsActive,
    u.HasSystemAccess,
    u.CreatedAt,
    u.LastLoginAt,
    o.OutletName,
    u.OutletId
FROM Users u
JOIN Roles r ON u.RoleId = r.Id
LEFT JOIN Outlet o ON o.Id = u.OutletId
WHERE r.RoleCode = @RoleCode
AND u.IsActive = 1
";

        public const string GetById = @"
SELECT
    u.*,
    r.RoleName,
    r.RoleCode,
    o.OutletName
FROM Users u
JOIN Roles r ON u.RoleId = r.Id
LEFT JOIN Outlet o ON o.Id = u.OutletId
WHERE u.Id = @Id and  u.IsActive =1
";

        public const string Create = @"
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
    HasSystemAccess,
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
    @HasSystemAccess,
    @CreatedAt,
    @OutletId
);

SELECT CAST(SCOPE_IDENTITY() AS INT);
";

        public const string UpdateLastLogin = "UPDATE Users SET LastLoginAt = GETUTCDATE() WHERE Id = @Id";

        public const string GetAll = @"
SELECT
    u.*,
    r.RoleName,
    r.RoleCode,
    o.OutletName
FROM Users u
JOIN Roles r ON u.RoleId = r.Id
LEFT JOIN Outlet o ON o.Id = u.OutletId

--where u.IsActive =1
";

        public const string Update = @"
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
    HasSystemAccess = @HasSystemAccess,
    OutletId   = @OutletId
WHERE Id = @Id
";

        public const string Delete = "DELETE FROM Users WHERE Id = @Id";

        public const string ChangePassword = @"UPDATE Users
                SET PasswordHash = @PasswordHash
                WHERE Id = @Id";

        public const string ResetPassword = @"UPDATE Users
                SET PasswordHash = @PasswordHash
                WHERE Id = @Id";
    }
}
