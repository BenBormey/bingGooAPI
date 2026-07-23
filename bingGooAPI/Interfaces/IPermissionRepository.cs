using JuJuBiAPI.Entities;

namespace JuJuBiAPI.Interfaces
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetAllAsync();

        Task<IEnumerable<int>> GetRolePermissionIdsAsync(int roleId);

        Task SaveRolePermissionsAsync(int roleId, List<int> permissionIds);

        Task<IEnumerable<string>> GetPermissionCodesByRoleCodeAsync(string roleCode);

        Task<bool> HasPermissionAsync(string roleCode, string permissionCode);
    }
}
