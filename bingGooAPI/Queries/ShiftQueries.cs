namespace JuJuBiAPI.Queries
{
    public static class ShiftQueries
    {
        public const string GetOpenShift = @"
                SELECT TOP 1 *
                FROM Shifts
                WHERE OutletId = @OutletId AND UserId = @UserId AND Status = 'Open'
                ORDER BY OpenedAt DESC;";

        public const string Open = @"
                INSERT INTO Shifts (OutletId, UserId, OpeningFloat, OpenedAt, Status)
                VALUES (@OutletId, @UserId, @OpeningFloat, GETDATE(), 'Open');

                SELECT * FROM Shifts WHERE ShiftID = CAST(SCOPE_IDENTITY() AS INT);";

        // Voided orders are excluded from sales; their money went back.
        public const string GetSummary = @"
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

        public const string Close = @"
                UPDATE Shifts
                SET Status       = 'Closed',
                    ClosedAt     = GETDATE(),
                    ExpectedCash = @ExpectedCash,
                    CountedCash  = @CountedCash,
                    Variance     = @CountedCash - @ExpectedCash,
                    Notes        = @Notes
                WHERE ShiftID = @ShiftID AND Status = 'Open';

                SELECT * FROM Shifts WHERE ShiftID = @ShiftID;";
    }
}
