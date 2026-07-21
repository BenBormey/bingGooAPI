using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
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
            return await _connection.QueryAsync<UOM>(UomQueries.GetAll);
        }

        public async Task<UOM?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<UOM>(UomQueries.GetById, new { Id = id });
        }

        public async Task<int> CreateAsync(UOM uom)
        {
            return await _connection.ExecuteScalarAsync<int>(UomQueries.Create, uom);
        }

        public async Task<bool> UpdateAsync(UOM uom)
        {
            var rows = await _connection.ExecuteAsync(UomQueries.Update, uom);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _connection.ExecuteAsync(UomQueries.Delete, new { Id = id });

            return rows > 0;
        }
    }
}
