using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
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
            var sql = "SELECT * FROM BankSetup ORDER BY BankName";

            var data = await _connection.QueryAsync<BankSetup>(sql);

            return data.ToList();
        }

        public async Task<BankSetup?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<BankSetup>(
                "SELECT * FROM BankSetup WHERE BankId=@id",
                new { id });
        }

        public async Task<int> CreateAsync(BankSetup bank)
        {
            var sql = @"

INSERT INTO BankSetup
(
BankCode,
BankName,
BankType,
MerchantId,
MerchantName,
ApiUrl,
ApiKey,
ApiSecret,
CallbackUrl,
Currency,
IsDefault,
IsActive
)

VALUES
(
@BankCode,
@BankName,
@BankType,
@MerchantId,
@MerchantName,
@ApiUrl,
@ApiKey,
@ApiSecret,
@CallbackUrl,
@Currency,
@IsDefault,
@IsActive
)

SELECT CAST(SCOPE_IDENTITY() AS INT);
";

            return await _connection.ExecuteScalarAsync<int>(sql, bank);
        }

        public async Task<bool> UpdateAsync(BankSetup bank)
        {
            var sql = @"

UPDATE BankSetup

SET

BankCode=@BankCode,
BankName=@BankName,
BankType=@BankType,
MerchantId=@MerchantId,
MerchantName=@MerchantName,
ApiUrl=@ApiUrl,
ApiKey=@ApiKey,
ApiSecret=@ApiSecret,
CallbackUrl=@CallbackUrl,
Currency=@Currency,
IsDefault=@IsDefault,
IsActive=@IsActive,
UpdatedAt=GETDATE()

WHERE BankId=@BankId";

            return await _connection.ExecuteAsync(sql, bank) > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _connection.ExecuteAsync(
                "DELETE FROM BankSetup WHERE BankId=@id",
                new { id }) > 0;
        }
    }
}
