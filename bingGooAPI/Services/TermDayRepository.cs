using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Term;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
{
    public class TermDayRepository : ITermDayRepository
    {
        private readonly IDbConnection _connection;

        public TermDayRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<TermDayEntity>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    Id,
                    CAST(CountDay AS VARCHAR(10)) + ' Day' AS TermDay,
                    CountDay
                FROM [DBJuJuBi].[dbo].[tblTermDay]
                ORDER BY Id DESC;
            ";

            return await _connection.QueryAsync<TermDayEntity>(sql);
        }

        public async Task<TermDayEntity?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT
                    Id,
                    CAST(CountDay AS VARCHAR(10)) + ' Day' AS TermDay,
                    CountDay
                FROM [DBJuJuBi].[dbo].[tblTermDay]
                WHERE Id = @Id;
            ";

            return await _connection.QueryFirstOrDefaultAsync<TermDayEntity>(sql, new { Id = id });
        }

        public async Task<TermDayEntity> AddAsync(CreateTermDayDto dto)
        {
            var sql = @"
                INSERT INTO [DBJuJuBi].[dbo].[tblTermDay]
                (
                    CountDay
                )
                VALUES
                (
                    @CountDay
                );

                SELECT
                    Id,
                    CAST(CountDay AS VARCHAR(10)) + ' Day' AS TermDay,
                    CountDay
                FROM [DBJuJuBi].[dbo].[tblTermDay]
                WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _connection.QuerySingleAsync<TermDayEntity>(sql, dto);
        }

        public async Task<bool> UpdateAsync(UpdateTermDayDto dto)
        {
            var sql = @"
                UPDATE [DBJuJuBi].[dbo].[tblTermDay]
                SET CountDay = @CountDay
                WHERE Id = @Id;
            ";

            var rows = await _connection.ExecuteAsync(sql, dto);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM [DBJuJuBi].[dbo].[tblTermDay] WHERE Id = @Id;";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
