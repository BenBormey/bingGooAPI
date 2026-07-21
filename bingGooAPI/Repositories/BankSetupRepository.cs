using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class BankSetupRepository : IBankSetupRepository
    {
        private readonly IDbConnection _connection;

        public BankSetupRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<BankSetup>> GetAllAsync()
        {
            var data = await _connection.QueryAsync<BankSetup>(BankSetupQueries.GetAll);

            return data.ToList();
        }

        public async Task<BankSetup?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<BankSetup>(
                BankSetupQueries.GetById,
                new { id });
        }

        public async Task<int> CreateAsync(BankSetup bank)
        {
            return await _connection.ExecuteScalarAsync<int>(BankSetupQueries.Create, bank);
        }

        public async Task<bool> UpdateAsync(BankSetup bank)
        {
            return await _connection.ExecuteAsync(BankSetupQueries.Update, bank) > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _connection.ExecuteAsync(
                BankSetupQueries.Delete,
                new { id }) > 0;
        }
    }
}
