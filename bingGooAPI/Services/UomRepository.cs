using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
{
    public class UomRepository : IUomRepository
    {
        private readonly IDbConnection _connection;

        public UomRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<UOM>> GetAllAsync()
        {
            const string sql = @"
                SELECT
                    UOMId,
                    UOMCode,
                    UOMName,
                    IsActive
                FROM UOM
                ORDER BY UOMCode;";

            return await _connection.QueryAsync<UOM>(sql);
        }

        public async Task<UOM?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT
                    UOMId,
                    UOMCode,
                    UOMName,
                    IsActive
                FROM UOM
                WHERE UOMId = @Id;";

            return await _connection.QueryFirstOrDefaultAsync<UOM>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(UOM uom)
        {
            const string sql = @"
                INSERT INTO UOM
                (
                    UOMCode,
                    UOMName,
                    IsActive
                )
                VALUES
                (
                    @UOMCode,
                    @UOMName,
                    @IsActive
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await _connection.ExecuteScalarAsync<int>(sql, uom);
        }

        public async Task<bool> UpdateAsync(UOM uom)
        {
            const string sql = @"
                UPDATE UOM
                SET
                    UOMCode = @UOMCode,
                    UOMName = @UOMName,
                    IsActive = @IsActive
                WHERE UOMId = @UOMId;";

            var rows = await _connection.ExecuteAsync(sql, uom);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"
                DELETE FROM UOM
                WHERE UOMId = @Id;";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });

            return rows > 0;
        }
    }
}