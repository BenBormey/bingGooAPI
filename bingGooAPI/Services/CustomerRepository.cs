using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Services
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
            const string sql = @"
                SELECT
                    CustomerID,
                    CustomerCode,
                    CustomerName,
                    Phone,
                    Email,
                    Address,
                    Points,
                    IsActive,
                    CreatedAt
                FROM Customer
                ORDER BY CustomerCode;";

            return await _connection.QueryAsync<Customer>(sql);
        }

        // Matches on phone, name or code so a cashier can type whichever the
        // customer gives them. Active members only, best matches first.
        public async Task<IEnumerable<Customer>> SearchAsync(string query)
        {
            const string sql = @"
                SELECT TOP 20
                    CustomerID,
                    CustomerCode,
                    CustomerName,
                    Phone,
                    Email,
                    Address,
                    Points,
                    IsActive,
                    CreatedAt
                FROM Customer
                WHERE IsActive = 1
                  AND (
                        Phone        LIKE @Like
                     OR CustomerName LIKE @Like
                     OR CustomerCode LIKE @Like
                  )
                ORDER BY
                    CASE WHEN Phone = @Exact THEN 0 ELSE 1 END,
                    CustomerName;";

            return await _connection.QueryAsync<Customer>(sql, new
            {
                Like = "%" + query + "%",
                Exact = query
            });
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT
                    CustomerID,
                    CustomerCode,
                    CustomerName,
                    Phone,
                    Email,
                    Address,
                    Points,
                    IsActive,
                    CreatedAt
                FROM Customer
                WHERE CustomerID = @Id;";

            return await _connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(Customer customer)
        {
            const string sql = @"
                INSERT INTO Customer
                (
                    CustomerCode,
                    CustomerName,
                    Phone,
                    Email,
                    Address,
                    Points,
                    IsActive
                )
                VALUES
                (
                    @CustomerCode,
                    @CustomerName,
                    @Phone,
                    @Email,
                    @Address,
                    @Points,
                    @IsActive
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await _connection.ExecuteScalarAsync<int>(sql, customer);
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            const string sql = @"
                UPDATE Customer
                SET
                    CustomerCode = @CustomerCode,
                    CustomerName = @CustomerName,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address,
                    Points = @Points,
                    IsActive = @IsActive
                WHERE CustomerID = @CustomerID;";

            var rows = await _connection.ExecuteAsync(sql, customer);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"
                DELETE FROM Customer
                WHERE CustomerID = @Id;";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });

            return rows > 0;
        }

        public async Task<bool> ExistsByCodeAsync(string customerCode, int? excludeId = null)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM Customer
                WHERE UPPER(CustomerCode) = UPPER(@CustomerCode)
                  AND (@ExcludeId IS NULL OR CustomerID <> @ExcludeId);";

            var count = await _connection.ExecuteScalarAsync<int>(
                sql,
                new { CustomerCode = customerCode, ExcludeId = excludeId });

            return count > 0;
        }

        public async Task<string> GetNextCodeAsync()
        {
            const string sql = "SELECT ISNULL(MAX(CustomerID), 0) + 1 FROM Customer;";

            var nextId = await _connection.ExecuteScalarAsync<int>(sql);
            return $"CUS-{nextId:0000}";
        }
    }
}
