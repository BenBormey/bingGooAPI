using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.ShelfLife;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
{
    public class ShelfLifeRepository : IShelfLifeRepository
    {
        private readonly IDbConnection _connection;

        public ShelfLifeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<ShelfLifeEntity>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    ShelfLifeId AS Id,
                    ShelfLifeName,
                    IsActive,
                    ShelfLifeValue,
                    ShelfLifeUnit
                FROM [DBJuJuBi].[dbo].[ShelfLife]
                ORDER BY ShelfLifeId DESC;
            ";

            return await _connection.QueryAsync<ShelfLifeEntity>(sql);
        }

        public async Task<ShelfLifeEntity?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT
                    ShelfLifeId AS Id,
                    ShelfLifeName,
                    IsActive,
                    ShelfLifeValue,
                    ShelfLifeUnit
                FROM [DBJuJuBi].[dbo].[ShelfLife]
                WHERE ShelfLifeId = @Id;
            ";

            return await _connection.QueryFirstOrDefaultAsync<ShelfLifeEntity>(sql, new { Id = id });
        }

        public async Task<ShelfLifeEntity> AddAsync(CreateShelfLifeDto dto)
        {
            var sql = @"
                INSERT INTO [DBJuJuBi].[dbo].[ShelfLife]
                (
                    ShelfLifeName,
                    IsActive,
                    ShelfLifeValue,
                    ShelfLifeUnit
                )
                VALUES
                (
                    @ShelfLifeName,
                    @IsActive,
                    @ShelfLifeValue,
                    @ShelfLifeUnit
                );

                SELECT
                    ShelfLifeId AS Id,
                    ShelfLifeName,
                    IsActive,
                    ShelfLifeValue,
                    ShelfLifeUnit
                FROM [DBJuJuBi].[dbo].[ShelfLife]
                WHERE ShelfLifeId = CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _connection.QuerySingleAsync<ShelfLifeEntity>(sql, dto);
        }

        public async Task<bool> UpdateAsync(UpdateShelfLifeDto dto)
        {
            var sql = @"
                UPDATE [DBJuJuBi].[dbo].[ShelfLife]
                SET
                    ShelfLifeName = @ShelfLifeName,
                    IsActive = @IsActive,
                    ShelfLifeValue = @ShelfLifeValue,
                    ShelfLifeUnit = @ShelfLifeUnit
                WHERE ShelfLifeId = @Id;
            ";

            var rows = await _connection.ExecuteAsync(sql, dto);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM [DBJuJuBi].[dbo].[ShelfLife] WHERE ShelfLifeId = @Id;";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
