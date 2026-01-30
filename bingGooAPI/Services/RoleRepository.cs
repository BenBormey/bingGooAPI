using bingGooAPI.Interfaces;
using bingGooAPI.Models.Role;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
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
            var sql = @"
        INSERT INTO Roles
        (
            RoleCode,
            RoleName,
            Description,
            IsSystemRole,
            IsActive,
            CreatedAt
        )
        VALUES
        (
            @RoleCode,
            @RoleName,
            @Description,
            @IsSystemRole,
            @IsActive,
            GETDATE()
        );
    ";

            return await _context.ExecuteAsync(sql, dto);
        }


        public Task<bool> DeleteAsync(int id)
        {
            var sqlDelete = @"
  update Roles set IsActive = 0
  where Id = @id";
            return _context.ExecuteAsync(sqlDelete, new { id }).ContinueWith(t => t.Result > 0);
        }

        public async Task<List<RoleDto>> GetAllAsync()
        {
            var sqlget = @"SELECT [Id]
      ,[RoleCode]
      ,[RoleName]
      ,[Description]
      ,[IsSystemRole]
      ,[IsActive]
      ,[CreatedAt]
      ,[UpdatedAt]
  FROM [DBAuthentication].[dbo].[Roles] where IsActive = 1
";
            var result = await _context.QueryAsync(sqlget);
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
            var sql = @"
        SELECT 
            Id,
            RoleCode,
            RoleName,
            Description,
            IsSystemRole,
            IsActive
        FROM Roles
        WHERE Id = @id and IsActive = 1
    ";

            return await _context.QuerySingleOrDefaultAsync<RoleDto>(
                sql,
                new { id }
            );
        }

        public Task<bool> UpdateAsync(int id, UpdateRoleDto dto)
        {
            var sqlupdate = @"  update Roles set 
  RoleCode = @RoleCode,
  RoleName = @RoleName,
  Description = @Description,
  IsSystemRole = @IsSystemRole,
  IsActive = @IsActive ,
  UpdatedAt = GETDATE()
  where Id = @id";
            return _context.ExecuteAsync(sqlupdate, new
            {
                dto.RoleCode,
                dto.RoleName,
                dto.Description,
                dto.IsSystemRole,
                dto.IsActive,
                id
            }).ContinueWith(t => t.Result > 0);



        }
    }
}
