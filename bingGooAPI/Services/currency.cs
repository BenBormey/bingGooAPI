using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;

namespace bingGooAPI.Services
{
    public class CurrencyService : IcurrencyRepository
    {
        private readonly IDbConnection _connection;

        public CurrencyService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Currency>> GetAllAsync()
        {
            var sql = "SELECT Id, CurrencyCode, CurrencyName, BuyRate, SellRate, IsBase, Active, CreatedAt FROM Currency";
            var result = await _connection.QueryAsync<Currency>(sql);
            return result;
        }

        public async Task<Currency?> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, CurrencyCode, CurrencyName, BuyRate, SellRate, IsBase, Active, CreatedAt FROM Currency WHERE Id = @Id";
            var result = await _connection.QueryFirstOrDefaultAsync<Currency>(sql, new { Id = id });
            return result;
        }

        public async Task<Currency> CreateAsync(Currency model)
        {
            try
            {
                var sql = @"
INSERT INTO Currency (CurrencyCode, CurrencyName, BuyRate, SellRate, IsBase, Active, CreatedAt)
VALUES (@CurrencyCode, @CurrencyName, @BuyRate, @SellRate, @IsBase, @Active, GETDATE());
SELECT CAST(SCOPE_IDENTITY() as int);";

                var id = await _connection.ExecuteScalarAsync<int>(sql, model);
                model.Id = id;
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task<bool> UpdateAsync(Currency model)
        {
            var sql = @"
UPDATE Currency
SET CurrencyCode = @CurrencyCode,
    CurrencyName = @CurrencyName,
    BuyRate = @BuyRate,
    SellRate = @SellRate,
    IsBase = @IsBase,
    Active = @Active
WHERE Id = @Id";

            var affected = await _connection.ExecuteAsync(sql, model);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Currency WHERE Id = @Id";
            var affected = await _connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }
    }
}
