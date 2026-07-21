using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly IDbConnection _connection;

        public SupplierRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Supplier> CreateAsync(Supplier supplier)
        {
            var id = await _connection.ExecuteScalarAsync<int>(SupplierQueries.Create, supplier);

            supplier.SupplierID = id;

            return supplier;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _connection.QueryAsync<Supplier>(SupplierQueries.GetAll);
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<Supplier>(
                SupplierQueries.GetById,
                new { Id = id });
        }

        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            var rows = await _connection.ExecuteAsync(SupplierQueries.Update, supplier);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _connection.ExecuteAsync(
                SupplierQueries.Delete,
                new { Id = id });

            return rows > 0;
        }

        public async Task<bool> ExistsByNameAsync(string supplierName)
        {
            var count = await _connection.ExecuteScalarAsync<int>(
                SupplierQueries.ExistsByName,
                new { SupplierName = supplierName });

            return count > 0;
        }

        public async Task<string> GetNextCodeAsync()
        {
            var nextId = await _connection.ExecuteScalarAsync<int>(SupplierQueries.GetNextCode);
            return $"SUP-{nextId:0000}";
        }
    }
}
