
using bingGooAPI.Models.Role;

namespace bingGooAPI.Interfaces
{
    public interface IRoleRepository
    {
  
        Task<List<RoleDto>> GetAllAsync();
        Task<RoleDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateRoleDto dto);
        Task<bool> UpdateAsync(int id, UpdateRoleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
