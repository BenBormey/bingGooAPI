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

            var id = await _connection.ExecuteScalarAsync<int>(sql, supplier);

            supplier.SupplierID = id;

            return supplier;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var sql = @"
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

            return await _connection.QueryAsync<Supplier>(sql);
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            var sql = @"
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

            return await _connection.QueryFirstOrDefaultAsync<Supplier>(
                sql,
                new { Id = id });
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

            var rows = await _connection.ExecuteAsync(sql, supplier);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"
        DELETE FROM Suppliers
        WHERE SupplierID = @Id
    ";

            var rows = await _connection.ExecuteAsync(
                sql,
                new { Id = id });

            return rows > 0;
        }

        public async Task<bool> ExistsByNameAsync(string supplierName)
        {
            var sql = @"
        SELECT COUNT(1)
        FROM dbo.Suppliers
        WHERE LOWER(SupplierName) = LOWER(@SupplierName)";

            var count = await _connection.ExecuteScalarAsync<int>(
                sql,
                new { SupplierName = supplierName });

            return count > 0;
        }
    }
}