using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
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
            var sql = @"
                INSERT INTO Suppliers
                (
                    SupplierCode,
                    SupplierName,
                    ContactName,
                    Phone,
                    Email,
                    Address,
                    TaxNumber,
                    Status,
                    CreatedAt
                )
                VALUES
                (
                    @SupplierCode,
                    @SupplierName,
                    @ContactName,
                    @Phone,
                    @Email,
                    @Address,
                    @TaxNumber,
                    @Status,
                    GETDATE()
                );

                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            var id = await _connection.ExecuteScalarAsync<int>(sql, supplier);

            supplier.SupplierID = id;

            return supplier;
        }


        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var sql = @"
                SELECT *
                FROM Suppliers
                WHERE Status = 1
                ORDER BY CreatedAt DESC
            ";

            return await _connection.QueryAsync<Supplier>(sql);
        }

  
        public async Task<Supplier?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT *
                FROM Suppliers
                WHERE SupplierID = @Id
            ";

            return await _connection.QueryFirstOrDefaultAsync<Supplier>(sql, new { Id = id });
        }


        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            var sql = @"
                UPDATE Suppliers
                SET
                    SupplierCode = @SupplierCode,
                    SupplierName = @SupplierName,
                    ContactName = @ContactName,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address,
                    TaxNumber = @TaxNumber,
                    Status = @Status
                WHERE SupplierID = @SupplierID
            ";

            var rows = await _connection.ExecuteAsync(sql, supplier);

            return rows > 0;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"
                UPDATE Suppliers
                SET Status = 0
                WHERE SupplierID = @Id
            ";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });

            return rows > 0;
        }
    }
}
