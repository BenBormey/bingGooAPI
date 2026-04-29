using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
{
    public class ProvinceRepository : IProvincesRepository
    {
        private readonly IDbConnection _connection;

        public ProvinceRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Provinces>> GetAllProvincesAsync()
        {
            var sql = @"
                SELECT 
                    ProvinceId, 
                    ProvinceNameKH, 
                    ProvinceNameEN, 
                    Code 
                FROM Provinces
                ORDER BY ProvinceId ASC";

            return await _connection.QueryAsync<Provinces>(sql);
        }

        public async Task<Provinces?> GetProvinceByIdAsync(int id)
        {
            var sql = @"
                SELECT 
                    ProvinceId, 
                    ProvinceNameKH, 
                    ProvinceNameEN, 
                    Code 
                FROM Provinces 
                WHERE ProvinceId = @Id";

            return await _connection.QueryFirstOrDefaultAsync<Provinces>(
                sql,
                new { Id = id }
            );
        }

        public async Task<Provinces> CreateAsync(Provinces model)
        {
            var sql = @"
                INSERT INTO Provinces (ProvinceNameKH, ProvinceNameEN, Code)
                VALUES (@ProvinceNameKH, @ProvinceNameEN, @Code);

                SELECT * FROM Provinces 
                WHERE ProvinceId = CAST(SCOPE_IDENTITY() AS INT);";

            return await _connection.QuerySingleAsync<Provinces>(sql, model);
        }

        public async Task<bool> UpdateAsync(Provinces model)
        {
            var sql = @"
                UPDATE Provinces
                SET 
                    ProvinceNameKH = @ProvinceNameKH,
                    ProvinceNameEN = @ProvinceNameEN,
                    Code = @Code
                WHERE ProvinceId = @ProvinceId";

            var rows = await _connection.ExecuteAsync(sql, model);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Provinces WHERE ProvinceId = @Id";
            var rows = await _connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}