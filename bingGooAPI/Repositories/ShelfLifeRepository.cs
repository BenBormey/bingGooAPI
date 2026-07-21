using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.ShelfLife;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
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
            return await _connection.QueryAsync<ShelfLifeEntity>(ShelfLifeQueries.GetAll);
        }

        public async Task<ShelfLifeEntity?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<ShelfLifeEntity>(ShelfLifeQueries.GetById, new { Id = id });
        }

        public async Task<ShelfLifeEntity> AddAsync(CreateShelfLifeDto dto)
        {
            return await _connection.QuerySingleAsync<ShelfLifeEntity>(ShelfLifeQueries.Add, dto);
        }

        public async Task<bool> UpdateAsync(UpdateShelfLifeDto dto)
        {
            var rows = await _connection.ExecuteAsync(ShelfLifeQueries.Update, dto);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _connection.ExecuteAsync(ShelfLifeQueries.Delete, new { Id = id });
            return rows > 0;
        }
    }
}
