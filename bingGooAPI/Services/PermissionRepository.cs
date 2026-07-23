using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Services
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IDbConnection _connection;

        public PermissionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            var sql = @"
                SELECT Id, PermissionCode, PermissionName, Remark
                FROM Permissions
                ORDER BY Id;";

            return await _connection.QueryAsync<Permission>(sql);
        }

        public async Task<IEnumerable<int>> GetRolePermissionIdsAsync(int roleId)
        {
            var sql = @"
                SELECT PermissionId
                FROM RolePermissions
                WHERE RoleId = @RoleId;";

            return await _connection.QueryAsync<int>(sql, new { RoleId = roleId });
        }

        public async Task SaveRolePermissionsAsync(int roleId, List<int> permissionIds)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var tran = _connection.BeginTransaction();

            await _connection.ExecuteAsync(
                "DELETE FROM RolePermissions WHERE RoleId = @RoleId;",
                new { RoleId = roleId },
                tran);

            if (permissionIds.Count > 0)
            {
                await _connection.ExecuteAsync(
                    "INSERT INTO RolePermissions (RoleId, PermissionId) VALUES (@RoleId, @PermissionId);",
                    permissionIds.Select(pid => new { RoleId = roleId, PermissionId = pid }),
                    tran);
            }

            tran.Commit();
        }

        public async Task<IEnumerable<string>> GetPermissionCodesByRoleCodeAsync(string roleCode)
        {
            var sql = @"
                SELECT p.PermissionCode
                FROM RolePermissions rp
                JOIN Roles r       ON r.Id = rp.RoleId
                JOIN Permissions p ON p.Id = rp.PermissionId
                WHERE r.RoleCode = @RoleCode;";

            return await _connection.QueryAsync<string>(sql, new { RoleCode = roleCode });
        }

        public async Task<bool> HasPermissionAsync(string roleCode, string permissionCode)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM RolePermissions rp
                JOIN Roles r       ON r.Id = rp.RoleId
                JOIN Permissions p ON p.Id = rp.PermissionId
                WHERE r.RoleCode = @RoleCode
                  AND p.PermissionCode = @PermissionCode;";

            var count = await _connection.ExecuteScalarAsync<int>(
                sql, new { RoleCode = roleCode, PermissionCode = permissionCode });

            return count > 0;
        }
    }
}
