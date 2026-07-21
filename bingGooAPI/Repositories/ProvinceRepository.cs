using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
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
            return await _connection.QueryAsync<Provinces>(ProvinceQueries.GetAll);
        }

        public async Task<Provinces?> GetProvinceByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<Provinces>(
                ProvinceQueries.GetById,
                new { Id = id }
            );
        }

        public async Task<Provinces> CreateAsync(Provinces model)
        {
            return await _connection.QuerySingleAsync<Provinces>(ProvinceQueries.Create, model);
        }

        public async Task<bool> UpdateAsync(Provinces model)
        {
            var rows = await _connection.ExecuteAsync(ProvinceQueries.Update, model);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _connection.ExecuteAsync(ProvinceQueries.Delete, new { Id = id });
            return rows > 0;
        }
    }
}
