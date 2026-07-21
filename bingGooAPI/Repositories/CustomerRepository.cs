using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDbConnection _connection;

        public CustomerRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _connection.QueryAsync<Customer>(CustomerQueries.GetAll);
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string query)
        {
            return await _connection.QueryAsync<Customer>(CustomerQueries.Search, new
            {
                Like = "%" + query + "%",
                Exact = query
            });
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<Customer>(CustomerQueries.GetById, new { Id = id });
        }

        public async Task<int> CreateAsync(Customer customer)
        {
            return await _connection.ExecuteScalarAsync<int>(CustomerQueries.Create, customer);
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            var rows = await _connection.ExecuteAsync(CustomerQueries.Update, customer);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _connection.ExecuteAsync(CustomerQueries.Delete, new { Id = id });

            return rows > 0;
        }

        public async Task<bool> ExistsByCodeAsync(string customerCode, int? excludeId = null)
        {
            var count = await _connection.ExecuteScalarAsync<int>(
                CustomerQueries.ExistsByCode,
                new { CustomerCode = customerCode, ExcludeId = excludeId });

            return count > 0;
        }

        public async Task<string> GetNextCodeAsync()
        {
            var nextId = await _connection.ExecuteScalarAsync<int>(CustomerQueries.GetNextCode);
            return $"CUS-{nextId:0000}";
        }
    }
}
