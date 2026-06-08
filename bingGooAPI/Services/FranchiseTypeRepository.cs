using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models; // ឬ bingGooAPI.Models.DTO ទៅតាមទីតាំងជាក់ស្តែង
using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace bingGooAPI.Services
{
    public class FranchiseTypeRepository : IFranchiseTypeItemRepository
    {
        private readonly IDbConnection _connection;

        public FranchiseTypeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<FranchiseType> CreateAsync(FranchiseType model)
        {
            var sql = @"
                INSERT INTO franchise_type
                    (TypeName, Description, IsActive, CreatedDate)
                VALUES
                    (@TypeName, @Description, @IsActive, GETDATE());

                SELECT Id, TypeName, Description, IsActive, CreatedDate 
                FROM franchise_type
                WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _connection.QuerySingleAsync<FranchiseType>(sql, model);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"
                DELETE FROM franchise_type
                WHERE Id = @Id
            ";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });

            return rows > 0;
        }

        public async Task<IEnumerable<FranchiseType>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    Id,
                    TypeName,
                    Description,
                    IsActive,
                    CreatedDate
                FROM franchise_type
                ORDER BY Id DESC
            ";

            return await _connection.QueryAsync<FranchiseType>(sql);
        }

        public async Task<FranchiseType?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT
                    Id,
                    TypeName,
                    Description,
                    IsActive,
                    CreatedDate
                FROM franchise_type
                WHERE Id = @Id
            ";

            return await _connection.QueryFirstOrDefaultAsync<FranchiseType>(
                sql,
                new { Id = id }
            );
        }

        public async Task<bool> UpdateAsync(FranchiseType model)
        {
            var sql = @"
                UPDATE franchise_type
                SET
                    TypeName = @TypeName,
                    Description = @Description,
                    IsActive = @IsActive
                WHERE Id = @Id
            ";

            var rows = await _connection.ExecuteAsync(sql, model);

            return rows > 0;
        }
    }
}