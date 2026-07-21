using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.HouseOpration;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
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
            return await _connection.QuerySingleAsync<HourOperation>(HourOperationQueries.Add, dto);
        }

        public async Task<IEnumerable<HourOperation>> GetAllAsync()
        {
            return await _connection.QueryAsync<HourOperation>(HourOperationQueries.GetAll);
        }

        public async Task<HourOperation?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<HourOperation>(
                HourOperationQueries.GetById,
                new { Id = id });
        }

        public async Task<bool> UpdateAsync(UpdateHourOperationDto dto)
        {
            var rows = await _connection.ExecuteAsync(HourOperationQueries.Update, dto);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _connection.ExecuteAsync(HourOperationQueries.Delete, new { Id = id });

            return rows > 0;
        }
    }
}
