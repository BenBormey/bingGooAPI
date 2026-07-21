using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JuJuBiAPI.Repositories
{
    public class FranchiseTypeRepository : IFranchiseTypeItemRepository
    {
        private readonly IDbConnection _connection;

        public FranchiseTypeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<FranchiseType> CreateAsync(FranchiseType model)
        {
            return await _connection.QuerySingleAsync<FranchiseType>(FranchiseTypeQueries.Create, model);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _connection.ExecuteAsync(FranchiseTypeQueries.Delete, new { Id = id });

            return rows > 0;
        }

        public async Task<IEnumerable<FranchiseType>> GetAllAsync()
        {
            return await _connection.QueryAsync<FranchiseType>(FranchiseTypeQueries.GetAll);
        }

        public async Task<FranchiseType?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<FranchiseType>(
                FranchiseTypeQueries.GetById,
                new { Id = id }
            );
        }

        public async Task<bool> UpdateAsync(FranchiseType model)
        {
            var rows = await _connection.ExecuteAsync(FranchiseTypeQueries.Update, model);

            return rows > 0;
        }
    }
}
