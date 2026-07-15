using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.HouseOpration;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Services
{
    public class HourOperationRepository : IHourOperationRepository
    {
        private readonly IDbConnection _connection;

        public HourOperationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<HourOperation> AddAsync(CreateHourOperationDto dto)
        {
            var sql = @"
                INSERT INTO DBJuJuBi.dbo.HourOperation
                (
               
                    OpenTime,
                    CloseTime,
                    Is24Hours,
                    Status,
                    CreatedAt
                )
                VALUES
                (
            
                    @OpenTime,
                    @CloseTime,
                    @Is24Hours,
                    @Status,
                    GETDATE()
                );

                SELECT *
                FROM DBJuJuBi.dbo.HourOperation
                WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _connection.QuerySingleAsync<HourOperation>(sql, dto);
        }

        public async Task<IEnumerable<HourOperation>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    Id,
                        OpenTime,
                    CloseTime,
                    Is24Hours,
                    Status,
                    CreatedAt
                FROM DBJuJuBi.dbo.HourOperation
                ORDER BY Id DESC;
            ";

            return await _connection.QueryAsync<HourOperation>(sql);
        }

        public async Task<HourOperation?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT
                    Id,
                  
                    OpenTime,
                    CloseTime,
                    Is24Hours,
                    Status,
                    CreatedAt
                FROM DBJuJuBi.dbo.HourOperation
                WHERE Id = @Id;
            ";

            return await _connection.QueryFirstOrDefaultAsync<HourOperation>(
                sql,
                new { Id = id });
        }

        public async Task<bool> UpdateAsync(UpdateHourOperationDto dto)
        {
            var sql = @"
                UPDATE DBJuJuBi.dbo.HourOperation
                SET
             
                    OpenTime = @OpenTime,
                    CloseTime = @CloseTime,
                    Is24Hours = @Is24Hours,
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id;
            ";

            var rows = await _connection.ExecuteAsync(sql, dto);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"
                DELETE FROM DBJuJuBi.dbo.HourOperation
                WHERE Id = @Id;
            ";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });

            return rows > 0;
        }
    }
}