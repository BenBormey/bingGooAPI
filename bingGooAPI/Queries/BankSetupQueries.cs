namespace JuJuBiAPI.Queries
{
    public static class BankSetupQueries
    {
        public const string GetAll = "SELECT * FROM BankSetup ORDER BY BankName";

        public const string GetById = "SELECT * FROM BankSetup WHERE BankId=@id";

        public const string Create = @"

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

        public const string Update = @"

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

        public const string Delete = "DELETE FROM BankSetup WHERE BankId=@id";
    }
}
