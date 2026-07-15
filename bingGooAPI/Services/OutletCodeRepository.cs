using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Outlet;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Services
{
    public class OutletCodeRepository : IOutletCodeRepository
    {
        private readonly IDbConnection _connection;

        public OutletCodeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<OutletCodeEntity>> GetAllAsync()
        {
            var sql = @"
                SELECT Id, OutletCode, IsActive
                FROM [DBJuJuBi].[dbo].[OutletCode]
                ORDER BY OutletCode DESC;
            ";

            return await _connection.QueryAsync<OutletCodeEntity>(sql);
        }

        public async Task<OutletCodeEntity?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT Id, OutletCode, IsActive
                FROM [DBJuJuBi].[dbo].[OutletCode]
                WHERE Id = @Id;
            ";

            return await _connection.QueryFirstOrDefaultAsync<OutletCodeEntity>(sql, new { Id = id });
        }

        public async Task<OutletCodeEntity> AddAsync(CreateOutletCodeDto dto)
        {
            var sql = @"
                INSERT INTO [DBJuJuBi].[dbo].[OutletCode]
                (
                    OutletCode,
                    IsActive
                )
                VALUES
                (
                    @OutletCode,
                    @IsActive
                );

                SELECT Id, OutletCode, IsActive
                FROM [DBJuJuBi].[dbo].[OutletCode]
                WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _connection.QuerySingleAsync<OutletCodeEntity>(sql, dto);
        }

        public async Task<bool> UpdateAsync(UpdateOutletCodeDto dto)
        {
            var sql = @"
                UPDATE [DBJuJuBi].[dbo].[OutletCode]
                SET OutletCode = @OutletCode,
                    IsActive = @IsActive
                WHERE Id = @Id;
            ";

            var rows = await _connection.ExecuteAsync(sql, dto);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM [DBJuJuBi].[dbo].[OutletCode] WHERE Id = @Id;";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }

        public async Task<bool> ExistsAsync(string outletCode, int? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM [DBJuJuBi].[dbo].[OutletCode]
                WHERE UPPER(OutletCode) = UPPER(@OutletCode)
                AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
            ";

            var count = await _connection.ExecuteScalarAsync<int>(sql, new { OutletCode = outletCode, ExcludeId = excludeId });
            return count > 0;
        }

        public async Task<string> GetNextCodeAsync()
        {
            var sql = "SELECT ISNULL(MAX(Id), 0) + 1 FROM [DBJuJuBi].[dbo].[OutletCode];";

            var nextId = await _connection.ExecuteScalarAsync<int>(sql);
            return $"UNT-{nextId}";
        }
    }
}
