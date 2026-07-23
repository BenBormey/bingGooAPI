using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class FranchiseRepository : IFranchiseRepository
    {
        private readonly IDbConnection _connection;

        public FranchiseRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Franchise>> GetAllAsync()
        {
            var result = await _connection.QueryAsync<Franchise>(FranchiseQueries.GetAll);
            return result.AsList();
        }

        public async Task<Franchise?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<Franchise>(
                FranchiseQueries.GetById,
                new { Id = id }
            );
        }

        public async Task<int> InsertAsync(Franchise franchise)
        {
            return await _connection.ExecuteAsync(FranchiseQueries.Insert, franchise);
        }

        public async Task<int> UpdateAsync(Franchise franchise)
        {
            return await _connection.ExecuteAsync(FranchiseQueries.Update, franchise);
        }

        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                var affected = await _connection.ExecuteAsync(FranchiseQueries.Delete, new { Id = id });

                if (affected == 0)
                {
                    // No row matched the given id
                    throw new InvalidOperationException("Franchise not found or already deleted.");
                }

                return affected;
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                // 547 = foreign key / reference constraint violation
                throw new InvalidOperationException(
                    "Cannot delete this franchise. It is still linked to an outlet. " +
                    "Please delete or update the related outlet first.");
            }
        }
    }
}
