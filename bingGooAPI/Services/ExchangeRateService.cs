using System.Data;
using Dapper;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;

namespace bingGooAPI.Services
{
    public class ExchangeRateService : IExchangeRateRepository
    {
        private readonly IDbConnection _connection;
           //private readonly IDbConnection _connection;
        public ExchangeRateService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<ExchangeRate>> GetAllAsync()
        {
            var sql = @"SELECT Id, CurrencyCode, Rate, Ask, Bid, Average, RateDate, Note, CreatedDate, CreatedBy
                        FROM ExchangeRate
                        ORDER BY RateDate DESC";

            return await _connection.QueryAsync<ExchangeRate>(sql);
        }

        public async Task<ExchangeRate?> GetByIdAsync(int id)
        {
            var sql = @"SELECT Id, CurrencyCode, Rate, Ask, Bid, Average, RateDate, Note, CreatedDate, CreatedBy
                        FROM ExchangeRate
                        WHERE Id = @Id";

            return await _connection.QueryFirstOrDefaultAsync<ExchangeRate>(sql, new { Id = id });
        }

//        public async Task<ExchangeRate> CreateAsync(ExchangeRate model)
//        {
//            using var transaction = _connection.BeginTransaction();

//            try
//            {
               
//                var insertSql = @"
//INSERT INTO ExchangeRate 
//(CurrencyCode, Rate, Ask, Bid, Average, RateDate, Note, CreatedBy)
//VALUES 
//(@CurrencyCode, @Rate, @Ask, @Bid, @Average, @RateDate, @Note, @CreatedBy);

//SELECT CAST(SCOPE_IDENTITY() as int);";

//                var id = await _connection.ExecuteScalarAsync<int>(insertSql, model, transaction);
//                model.Id = id;

               
//                var updateSql = @"
//UPDATE Currency
//SET 
//    BuyRate = @Bid,
//    SellRate = @Ask
//WHERE CurrencyCode = @CurrencyCode";

//                await _connection.ExecuteAsync(updateSql, new
//                {
//                    model.CurrencyCode,
//                    model.Bid,
//                    model.Ask
//                }, transaction);

//                transaction.Commit();
//                return model;
//            }
//            catch
//            {
//                transaction.Rollback();
//                throw;
//            }
//        }
        public async Task<ExchangeRate> CreateAsync(ExchangeRate model)
        {
            try
            {
                // 🔥 IMPORTANT: open connection
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                // 1. Insert ExchangeRate
                var insertSql = @"
INSERT INTO ExchangeRate 
(CurrencyCode, Rate, Ask, Bid, Average, RateDate, Note, CreatedBy)
VALUES 
(@CurrencyCode, @Rate, @Ask, @Bid, @Average, @RateDate, @Note, @CreatedBy);

SELECT CAST(SCOPE_IDENTITY() as int);";

                var id = await _connection.ExecuteScalarAsync<int>(insertSql, model);
                model.Id = id;

                // 2. Update Currency
                var updateSql = @"
UPDATE Currency
SET 
    BuyRate = @Bid,
    SellRate = @Ask
WHERE CurrencyCode = @CurrencyCode";

                await _connection.ExecuteAsync(updateSql, new
                {
                    model.CurrencyCode,
                    model.Bid,
                    model.Ask
                });

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _connection.Close(); // 🔥 good practice
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM ExchangeRate WHERE Id = @Id";
            var affected = await _connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }
    }
}