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
    ISNULL(SUM(ps.StockQty * pr.CostPrice), 0)
FROM ProductStocks ps
JOIN Products pr ON ps.ProductID = pr.ProductID

    ";

        public const string BalanceSheetRetainedEarnings = @"
        SELECT
            ISNULL(SUM(p.AmountPaid),0)
          - ISNULL(SUM(oi.Quantity * pr.CostPrice),0)
        FROM Orders o
        JOIN Payments p ON o.OrderID = p.OrderID
        JOIN OrderItems oi ON o.OrderID = oi.OrderID
        JOIN Products pr ON oi.ProductID = pr.ProductID
        WHERE o.OrderStatus = 'Paid'
          AND o.CreatedAt <= @AsOfDate
    ";

        public const string PnL = @"
                SELECT


                    SUM(p.AmountPaid) AS TotalSales,

                    SUM(oi.Quantity * pr.CostPrice) AS TotalCost,

                    SUM(p.AmountPaid)
                      - SUM(oi.Quantity * pr.CostPrice) AS Profit

                FROM Orders o
                JOIN Payments p ON o.OrderID = p.OrderID
                JOIN OrderItems oi ON o.OrderID = oi.OrderID
                JOIN Products pr ON oi.ProductID = pr.ProductID

                WHERE
                    o.OrderStatus = 'Paid'
                    AND o.CreatedAt BETWEEN @From AND @To



            ";

        // Stored procedure name for the sales-by-outlet report.
        public const string SalesReportProc = "rptSaleReportByOutlet";
    }
}
