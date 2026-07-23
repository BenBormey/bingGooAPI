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

        // One batch feeding the whole MD dashboard — 7 result sets, read with
        // QueryMultiple. Cancelled orders are excluded everywhere.
        // NOTE: OrderItems.ProductID is an int holding ProNumY-as-number, so
        // the product join goes through TRY_CAST.
        public const string Dashboard = @"
            DECLARE @today DATE = CAST(GETDATE() AS DATE);
            DECLARE @monthStart DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

            -- 1. headline totals
            SELECT
              ISNULL(SUM(CASE WHEN CAST(CreatedAt AS DATE) = @today THEN GrandTotal END), 0) AS SalesToday,
              ISNULL(SUM(CASE WHEN CAST(CreatedAt AS DATE) = DATEADD(DAY, -1, @today) THEN GrandTotal END), 0) AS SalesYesterday,
              ISNULL(SUM(CASE WHEN CreatedAt >= DATEADD(DAY, -6, @today) THEN GrandTotal END), 0) AS SalesWeek,
              ISNULL(SUM(CASE WHEN CreatedAt >= @monthStart THEN GrandTotal END), 0) AS SalesMonth,
              SUM(CASE WHEN CAST(CreatedAt AS DATE) = @today THEN 1 ELSE 0 END) AS OrdersToday,
              ISNULL(SUM(CASE WHEN CreatedAt >= @monthStart THEN TaxAmount END), 0) AS VatMonth,
              (SELECT ISNULL(SUM(os.StockQty * ISNULL(t.ProImpPri, 0)), 0)
               FROM OutletStock os
               LEFT JOIN TPRProducts t ON t.ProNumY = os.ProNumY) AS StockValue
            FROM Orders WHERE OrderStatus <> 'Cancelled';

            -- 2. sales by day, last 14 days
            SELECT CAST(CreatedAt AS DATE) AS [Date], SUM(GrandTotal) AS Total
            FROM Orders
            WHERE OrderStatus <> 'Cancelled' AND CreatedAt >= DATEADD(DAY, -13, @today)
            GROUP BY CAST(CreatedAt AS DATE)
            ORDER BY [Date];

            -- 3. sales by outlet, this month
            SELECT ISNULL(O.OutletName, 'Outlet ' + CAST(ORD.OutletID AS VARCHAR)) AS Name,
                   SUM(ORD.GrandTotal) AS Value
            FROM Orders ORD
            LEFT JOIN Outlet O ON O.Id = ORD.OutletID
            WHERE ORD.OrderStatus <> 'Cancelled' AND ORD.CreatedAt >= @monthStart
            GROUP BY ISNULL(O.OutletName, 'Outlet ' + CAST(ORD.OutletID AS VARCHAR))
            ORDER BY Value DESC;

            -- 4. top products by quantity, this month
            SELECT TOP 5 ISNULL(TP.ProName, CAST(OI.ProductID AS VARCHAR)) AS Name,
                   SUM(OI.Quantity) AS Value
            FROM OrderItems OI
            JOIN Orders ORD ON ORD.OrderID = OI.OrderID
                AND ORD.OrderStatus <> 'Cancelled' AND ORD.CreatedAt >= @monthStart
            LEFT JOIN TPRProducts TP ON TRY_CAST(TP.ProNumY AS BIGINT) = OI.ProductID
            GROUP BY ISNULL(TP.ProName, CAST(OI.ProductID AS VARCHAR))
            ORDER BY Value DESC;

            -- 5. sales by payment method, this month
            SELECT ISNULL(NULLIF(LTRIM(RTRIM(PaymentMethod)), ''), 'Unknown') AS Name,
                   SUM(GrandTotal) AS Value
            FROM Orders
            WHERE OrderStatus <> 'Cancelled' AND CreatedAt >= @monthStart
            GROUP BY ISNULL(NULLIF(LTRIM(RTRIM(PaymentMethod)), ''), 'Unknown')
            ORDER BY Value DESC;

            -- 6. low stock (under 10), worst first
            SELECT TOP 10 ISNULL(TP.ProName, OS.ProNumY) AS Product,
                   ISNULL(O.OutletName, CAST(OS.OutletId AS VARCHAR)) AS Outlet,
                   OS.StockQty
            FROM OutletStock OS
            LEFT JOIN TPRProducts TP ON TP.ProNumY = OS.ProNumY
            LEFT JOIN Outlet O ON O.Id = OS.OutletId
            WHERE OS.StockQty < 10
            ORDER BY OS.StockQty ASC;

            -- 7. restock requests waiting for approval
            SELECT COUNT(*) FROM OutletOrders WHERE Status = 'Requested';

            -- 8. recent outlet orders (newest first)
            SELECT TOP 7 o.OutletOrderNo,
                   ISNULL(ol.OutletName, CAST(o.OutletID AS VARCHAR)) AS Outlet,
                   o.OrderDate, o.ExpectedDate, o.Status, o.Note,
                   (SELECT COUNT(*) FROM OutletOrderItems i
                    WHERE i.OutletOrderID = o.OutletOrderID) AS Items
            FROM OutletOrders o
            LEFT JOIN Outlet ol ON ol.Id = o.OutletID
            ORDER BY o.CreatedAt DESC;";
    }
}
