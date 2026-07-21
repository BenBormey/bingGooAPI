using System.Data;
using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;

namespace JuJuBiAPI.Services
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly IDbConnection _connection;

        public ShiftRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Shift?> GetOpenShiftAsync(int outletId, int userId)
        {
            const string sql = @"
                SELECT TOP 1 *
                FROM Shifts
                WHERE OutletId = @OutletId AND UserId = @UserId AND Status = 'Open'
                ORDER BY OpenedAt DESC;";

            return await _connection.QueryFirstOrDefaultAsync<Shift>(
                sql, new { OutletId = outletId, UserId = userId });
        }

        public async Task<Shift> OpenAsync(int outletId, int userId, decimal openingFloat)
        {
            // One open shift per cashier per outlet — opening again while one is
            // open would orphan the first and break the reconciliation chain.
            var existing = await GetOpenShiftAsync(outletId, userId);
            if (existing != null)
                throw new InvalidOperationException(
                    $"Shift #{existing.ShiftID} is already open (since {existing.OpenedAt:HH:mm}). Close it first.");

            const string sql = @"
                INSERT INTO Shifts (OutletId, UserId, OpeningFloat, OpenedAt, Status)
                VALUES (@OutletId, @UserId, @OpeningFloat, GETDATE(), 'Open');

                SELECT * FROM Shifts WHERE ShiftID = CAST(SCOPE_IDENTITY() AS INT);";

            return await _connection.QuerySingleAsync<Shift>(
                sql, new { OutletId = outletId, UserId = userId, OpeningFloat = openingFloat });
        }

        public async Task<ShiftSummary?> GetSummaryAsync(int shiftId)
        {
            // Voided orders are excluded from sales; their money went back.
            const string sql = @"
                SELECT
                    S.ShiftID,
                    S.OpeningFloat,
                    S.OpenedAt,
                    COUNT(O.OrderID)                                                   AS OrdersTotal,
                    SUM(CASE WHEN O.OrderStatus IN ('Cancelled','Voided') THEN 1 ELSE 0 END) AS OrdersVoided,
                    SUM(CASE WHEN O.OrderStatus NOT IN ('Cancelled','Voided') AND O.PaymentMethod = 'Cash'     THEN O.GrandTotal ELSE 0 END) AS SalesCash,
                    SUM(CASE WHEN O.OrderStatus NOT IN ('Cancelled','Voided') AND O.PaymentMethod = 'Card'     THEN O.GrandTotal ELSE 0 END) AS SalesCard,
                    SUM(CASE WHEN O.OrderStatus NOT IN ('Cancelled','Voided') AND O.PaymentMethod = 'E-Wallet' THEN O.GrandTotal ELSE 0 END) AS SalesWallet,
                    SUM(CASE WHEN O.OrderStatus NOT IN ('Cancelled','Voided')                                  THEN O.GrandTotal ELSE 0 END) AS SalesTotal
                FROM Shifts S
                LEFT JOIN Orders O ON O.ShiftId = S.ShiftID
                WHERE S.ShiftID = @ShiftID
                GROUP BY S.ShiftID, S.OpeningFloat, S.OpenedAt;";

            var summary = await _connection.QueryFirstOrDefaultAsync<ShiftSummary>(
                sql, new { ShiftID = shiftId });

            if (summary != null)
                summary.ExpectedCash = summary.OpeningFloat + summary.SalesCash;

            return summary;
        }

        public async Task<Shift?> CloseAsync(int shiftId, decimal countedCash, string notes)
        {
            var summary = await GetSummaryAsync(shiftId);
            if (summary == null)
                return null;

            const string sql = @"
                UPDATE Shifts
                SET Status       = 'Closed',
                    ClosedAt     = GETDATE(),
                    ExpectedCash = @ExpectedCash,
                    CountedCash  = @CountedCash,
                    Variance     = @CountedCash - @ExpectedCash,
                    Notes        = @Notes
                WHERE ShiftID = @ShiftID AND Status = 'Open';

                SELECT * FROM Shifts WHERE ShiftID = @ShiftID;";

            return await _connection.QueryFirstOrDefaultAsync<Shift>(sql, new
            {
                ShiftID = shiftId,
                ExpectedCash = summary.ExpectedCash,
                CountedCash = countedCash,
                Notes = notes
            });
        }
    }
}
