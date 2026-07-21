using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Role;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnection _context;
        public RoleRepository(IDbConnection context)
        {
            this._context = context;

        }
        public async Task<int> CreateAsync(CreateRoleDto dto)
        {
            return await _context.ExecuteAsync(RoleQueries.Create, dto);
        }


        public Task<bool> DeleteAsync(int id)
        {
            return _context.ExecuteAsync(RoleQueries.Delete, new { id }).ContinueWith(t => t.Result > 0);
        }

        public async Task<List<RoleDto>> GetAllAsync()
        {
            var result = await _context.QueryAsync(RoleQueries.GetAll);
            return result.Select(r => new RoleDto
            {
                Id = r.Id,
                RoleCode = r.RoleCode,
                RoleName = r.RoleName,
                Description = r.Description,
                IsSystemRole = r.IsSystemRole,
                IsActive = r.IsActive
            }).ToList();
        }

        public async Task<RoleDto?> GetByIdAsync(int id)
        {
            return await _context.QuerySingleOrDefaultAsync<RoleDto>(
                RoleQueries.GetById,
                new { id }
            );
        }

        public Task<bool> UpdateAsync(int id, UpdateRoleDto dto)
        {
            return _context.ExecuteAsync(RoleQueries.Update, new
            {
                dto.RoleCode,
                dto.RoleName,
                dto.Description,
                dto.IsSystemRole,
                dto.IsActive,
                id
            }).ContinueWith(t => t.Result > 0);
        }

        public async Task<string> GetNextCodeAsync()
        {
            var nextId = await _context.ExecuteScalarAsync<int>(RoleQueries.GetNextCode);
            return $"ROL-{nextId:0000}";
        }
    }
}
