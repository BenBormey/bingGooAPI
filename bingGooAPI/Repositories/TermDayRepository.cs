using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Term;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
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
            return await _connection.QueryAsync<TermDayEntity>(TermDayQueries.GetAll);
        }

        public async Task<TermDayEntity?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<TermDayEntity>(TermDayQueries.GetById, new { Id = id });
        }

        public async Task<TermDayEntity> AddAsync(CreateTermDayDto dto)
        {
            return await _connection.QuerySingleAsync<TermDayEntity>(TermDayQueries.Add, dto);
        }

        public async Task<bool> UpdateAsync(UpdateTermDayDto dto)
        {
            var rows = await _connection.ExecuteAsync(TermDayQueries.Update, dto);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _connection.ExecuteAsync(TermDayQueries.Delete, new { Id = id });
            return rows > 0;
        }
    }
}
