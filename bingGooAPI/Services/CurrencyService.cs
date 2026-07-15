using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using Dapper;

namespace JuJuBiAPI.Services
{
    public class CurrencyService : ICurrencyRepository
    {
        private readonly IDbConnection _connection;

        public CurrencyService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Currency>> GetAllAsync()
        {
            var sql = @"
SELECT Id, CurrencyNo, CurrencyCode, CurrencyName, BuyRate, SellRate,
       SupplierId, IsBase, Active, CreatedAt, UpdatedAt
FROM Currency
ORDER BY Id";
            var result = await _connection.QueryAsync<Currency>(sql);
            return result;
        }

        public async Task<Currency?> GetByIdAsync(int id)
        {
            var sql = @"
SELECT Id, CurrencyNo, CurrencyCode, CurrencyName, BuyRate, SellRate,
       SupplierId, IsBase, Active, CreatedAt, UpdatedAt
FROM Currency
WHERE Id = @Id";
            var result = await _connection.QueryFirstOrDefaultAsync<Currency>(sql, new { Id = id });
            return result;
        }

        public async Task<Currency> CreateAsync(Currency model)
        {
            // Check duplicate CurrencyCode
            var exists = await _connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(*)
          FROM Currency
          WHERE UPPER(CurrencyCode) = UPPER(@CurrencyCode)",
                new { model.CurrencyCode });

            if (exists > 0)
            {
                throw new Exception($"Currency '{model.CurrencyCode}' already exists.");
            }

            const string sql = @"
    INSERT INTO Currency
    (
        CurrencyNo,
        CurrencyCode,
        CurrencyName,
        BuyRate,
        SellRate,
        SupplierId,
        IsBase,
        Active,
        CreatedAt
    )
    VALUES
    (
        @CurrencyNo,
        @CurrencyCode,
        @CurrencyName,
        @BuyRate,
        @SellRate,
        @SupplierId,
        @IsBase,
        @Active,
        GETDATE()
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var id = await _connection.ExecuteScalarAsync<int>(sql, model);

            model.Id = id;
            return model;
        }

        public async Task<bool> UpdateAsync(Currency model)
        {
            var exists = await _connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(*)
          FROM Currency
          WHERE UPPER(CurrencyCode) = UPPER(@CurrencyCode)
            AND Id <> @Id",
                new
                {
                    model.CurrencyCode,
                    model.Id
                });

            if (exists > 0)
            {
                throw new Exception($"Currency '{model.CurrencyCode}' already exists.");
            }

            const string sql = @"
    UPDATE Currency
    SET
        CurrencyNo   = @CurrencyNo,
        CurrencyCode = @CurrencyCode,
        CurrencyName = @CurrencyName,
        BuyRate      = @BuyRate,
        SellRate     = @SellRate,
        SupplierId   = @SupplierId,
        IsBase       = @IsBase,
        Active       = @Active,
        UpdatedAt    = GETDATE()
    WHERE Id = @Id;";

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