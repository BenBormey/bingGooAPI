using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Outlet;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
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
            return await _connection.QueryAsync<OutletCodeEntity>(OutletCodeQueries.GetAll);
        }

        public async Task<OutletCodeEntity?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<OutletCodeEntity>(OutletCodeQueries.GetById, new { Id = id });
        }

        public async Task<OutletCodeEntity> AddAsync(CreateOutletCodeDto dto)
        {
            return await _connection.QuerySingleAsync<OutletCodeEntity>(OutletCodeQueries.Add, dto);
        }

        public async Task<bool> UpdateAsync(UpdateOutletCodeDto dto)
        {
            var rows = await _connection.ExecuteAsync(OutletCodeQueries.Update, dto);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _connection.ExecuteAsync(OutletCodeQueries.Delete, new { Id = id });
            return rows > 0;
        }

        public async Task<bool> ExistsAsync(string outletCode, int? excludeId = null)
        {
            var count = await _connection.ExecuteScalarAsync<int>(OutletCodeQueries.Exists, new { OutletCode = outletCode, ExcludeId = excludeId });
            return count > 0;
        }

        public async Task<string> GetNextCodeAsync()
        {
            var nextId = await _connection.ExecuteScalarAsync<int>(OutletCodeQueries.GetNextCode);
            return $"UNT-{nextId}";
        }
    }
}
