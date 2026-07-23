namespace JuJuBiAPI.Queries
{
    public static class ReportQueries
    {
        public const string BalanceSheetCash = @"
        SELECT ISNULL(SUM(p.AmountPaid), 0)
        FROM Payments p
        JOIN Orders o ON p.OrderID = o.OrderID
        WHERE o.OrderStatus = 'Paid'
          AND o.CreatedAt <= @AsOfDate
    ";

        public const string BalanceSheetInventory = @"
SELECT
    ISNULL(SUM(os.StockQty * ISNULL(t.ProImpPri, 0)), 0)
FROM OutletStock os
JOIN TPRProducts t ON t.ProNumY = os.ProNumY

    ";

        // Sales and cost are aggregated in separate subqueries: joining
        // Payments and OrderItems together multiplies AmountPaid by the item
        // count (and cost by the payment count) on orders with several of each.
        public const string BalanceSheetRetainedEarnings = @"
        SELECT
            (SELECT ISNULL(SUM(p.AmountPaid), 0)
             FROM Orders o
             JOIN Payments p ON o.OrderID = p.OrderID
             WHERE o.OrderStatus = 'Paid'
               AND o.CreatedAt <= @AsOfDate)
          - (SELECT ISNULL(SUM(oi.Quantity * ISNULL(t.ProImpPri, 0)), 0)
             FROM Orders o
             JOIN OrderItems oi ON o.OrderID = oi.OrderID
             JOIN TPRProducts t ON t.ProNumY = oi.ProductID
             WHERE o.OrderStatus = 'Paid'
               AND o.CreatedAt <= @AsOfDate)
    ";

        // Same separation as BalanceSheetRetainedEarnings — sales and cost
        // must not share one join or both sums get inflated.
        public const string PnL = @"
                WITH Sales AS (
                    SELECT ISNULL(SUM(p.AmountPaid), 0) AS TotalSales
                    FROM Orders o
                    JOIN Payments p ON o.OrderID = p.OrderID
                    WHERE o.OrderStatus = 'Paid'
                      AND o.CreatedAt BETWEEN @From AND @To
                ),
                Cost AS (
                    SELECT ISNULL(SUM(oi.Quantity * ISNULL(t.ProImpPri, 0)), 0) AS TotalCost
                    FROM Orders o
                    JOIN OrderItems oi ON o.OrderID = oi.OrderID
                    JOIN TPRProducts t ON t.ProNumY = oi.ProductID
                    WHERE o.OrderStatus = 'Paid'
                      AND o.CreatedAt BETWEEN @From AND @To
                )
                SELECT
                    s.TotalSales,
                    c.TotalCost,
                    s.TotalSales - c.TotalCost AS Profit
                FROM Sales s CROSS JOIN Cost c;
            ";

        // Stored procedure name for the sales-by-outlet report.
        public const string SalesReportProc = "rptSaleReportByOutlet";
    }
}
