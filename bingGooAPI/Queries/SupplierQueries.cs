namespace JuJuBiAPI.Queries
{
    public static class SupplierQueries
    {
        public const string Create = @"
                INSERT INTO [DBJuJuBi].[dbo].[Suppliers]
                (
                    SupplierCode,
                    SupplierName,
                    ContactName,
                    Phone,
                    Email,
                    Address,
                    TaxNumber,
                    KhmerSupAddress,
                    Country,
                    FaxLine2,
                    Website,
                    LEAOTime,
                    Note,
                    ChequeName,
                    Term,
                    DayOrder,
                    CountryOfPurchase,
                    SetPercentOrderLevel,
                    VATTEMP,
                    Status,
                    CreatedAt,
                    SupplierNamekh,
TermId

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
                    @KhmerSupAddress,
                    @Country,
                    @FaxLine2,
                    @Website,
                    @LEAOTime,
                    @Note,
                    @ChequeName,
                    @Term,
                    @DayOrder,
                    @CountryOfPurchase,
                    @SetPercentOrderLevel,
                    @VATTEMP,
                    @Status,
                    GETDATE(),
                    @SupplierNamekh ,
@TermId
                );

                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

        public const string GetAll = @"
                SELECT
    s.SupplierID,
    s.SupplierCode,
    s.SupplierName,
    s.ContactName,
    s.Phone,
    s.Email,
    s.Address,
    s.TaxNumber,
    s.KhmerSupAddress,
    s.Country,
    s.FaxLine2,
    s.Website,
    s.LEAOTime,
    s.Note,
    s.ChequeName,
    s.Term,
    s.DayOrder,
    s.CountryOfPurchase,
    s.SetPercentOrderLevel,
    s.VATTEMP,
    s.Status,
    s.CreatedAt,
    s.SupplierNamekh,
    s.TermId,
    t.CountDay
FROM Suppliers AS s
LEFT JOIN tblTermDay AS t
    ON s.TermId = t.Id
ORDER BY s.CreatedAt DESC;
            ";

        public const string GetById = @"
              SELECT
    s.SupplierID,
    s.SupplierCode,
    s.SupplierName,
    s.ContactName,
    s.Phone,
    s.Email,
    s.Address,
    s.TaxNumber,
    s.KhmerSupAddress,
    s.Country,
    s.FaxLine2,
    s.Website,
    s.LEAOTime,
    s.Note,
    s.ChequeName,
    s.Term,
    s.DayOrder,
    s.CountryOfPurchase,
    s.SetPercentOrderLevel,
    s.VATTEMP,
    s.Status,
    s.CreatedAt,
    s.SupplierNamekh,
    s.TermId,
    t.CountDay
FROM Suppliers AS s
LEFT JOIN tblTermDay AS t
    ON s.TermId = t.Id


 WHERE s.SupplierID = @Id
ORDER BY s.CreatedAt DESC;

            ";

        public const string Update = @"
                UPDATE Suppliers
                SET
                    SupplierCode = @SupplierCode,
                    SupplierName = @SupplierName,
                    ContactName = @ContactName,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address,
                    TaxNumber = @TaxNumber,
                    KhmerSupAddress = @KhmerSupAddress,
                    Country = @Country,
                    FaxLine2 = @FaxLine2,
                    Website = @Website,
                    LEAOTime = @LEAOTime,
                    Note = @Note,
                    ChequeName = @ChequeName,
                    Term = @Term,
                    DayOrder = @DayOrder,
                    CountryOfPurchase = @CountryOfPurchase,
                    SetPercentOrderLevel = @SetPercentOrderLevel,
                    VATTEMP = @VATTEMP,
                    Status = @Status,
                    SupplierNamekh = @SupplierNamekh,
                    TermId  = @TermId
                WHERE SupplierID = @SupplierID
            ";

        public const string Delete = @"
        DELETE FROM Suppliers
        WHERE SupplierID = @Id
    ";

        public const string ExistsByName = @"
        SELECT COUNT(1)
        FROM dbo.Suppliers
        WHERE LOWER(SupplierName) = LOWER(@SupplierName)";

        public const string GetNextCode = "SELECT ISNULL(MAX([SupplierID]), 0) + 1 FROM [DBJuJuBi].[dbo].[Suppliers];";
    }
}
