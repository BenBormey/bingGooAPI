namespace JuJuBiAPI.Queries
{
    public static class OrderQueries
    {
        public const string CreateOrder = @"
                INSERT INTO Orders
                (
                    CartID,
                    UserID,
                    OutletID,
                    SubTotal,
                    DiscountAmount,
                    TaxAmount,
                    GrandTotal,
                    OrderStatus,
                    CreatedAt
                )
                VALUES
                (
                    @CartID,
                    @UserID,
                    @OutletID,
                    @SubTotal,
                    @DiscountAmount,
                    @TaxAmount,
                    @GrandTotal,
                    @OrderStatus,
                    GETDATE()
                );

                SELECT *
                FROM Orders
                WHERE OrderID = CAST(SCOPE_IDENTITY() AS INT);
            ";

        public const string AddOrderItem = @"
                INSERT INTO OrderItems
                (
                    OrderID,
                    ProductID,
                    Quantity,
                    UnitPrice,
                    DiscountPercent,
                    DiscountAmount,
                    TaxPercent,
                    TaxAmount,
                    SubTotal,
                    TotalPrice,
                    CreatedAt
                )
                VALUES
                (
                    @OrderID,
                    @ProductID,
                    @Quantity,
                    @UnitPrice,
                    @DiscountPercent,
                    @DiscountAmount,
                    @TaxPercent,
                    @TaxAmount,
                    @SubTotal,
                    @TotalPrice,
                    GETDATE()
                );
            ";

        public const string GetById = @"
                SELECT
                    O.*,
                    U.FullName  AS CashierName,
                    OT.OutletName
                FROM Orders O
                    LEFT JOIN Users U   ON U.Id = O.UserID
                    LEFT JOIN Outlet OT ON OT.Id = O.OutletID
                WHERE O.OrderID = @OrderID;

                SELECT
                    OI.*,
                    P.ProductName,
                    P.ProductCode,
                    P.ImageUrl
                FROM OrderItems OI
                    LEFT JOIN Products P ON P.ProductID = OI.ProductID
                WHERE OI.OrderID = @OrderID;
            ";

        public const string GetByUser = @"
                SELECT *
                FROM Orders
                WHERE UserID = @UserID
                ORDER BY CreatedAt DESC;
            ";

        public const string GetByOutlet = @"
                SELECT
                    O.*,
                    U.FullName  AS CashierName,
                    OT.OutletName
                FROM Orders O
                    LEFT JOIN Users U  ON U.Id = O.UserID
                    LEFT JOIN Outlet OT ON OT.Id = O.OutletID
                WHERE O.OutletID = @OutletID
                ORDER BY O.CreatedAt DESC;
            ";

        // --- PosCheckout helpers ---
        public const string GetOutletStock = @"
                    SELECT StockQty
                    FROM OutletStock
                    WHERE ProNumY = @ProNumY AND OutletId = @OutletId;";

        public const string GetProductName = "SELECT ProName FROM TPRProducts WHERE ProNumY = @ProNumY;";

        public const string GetCustomerPoints = "SELECT Points FROM Customer WHERE CustomerID = @Id AND IsActive = 1;";

        public const string InsertCart = @"
                INSERT INTO Carts
                    (UserID, OutletID, SubTotal, DiscountAmount, TaxAmount, GrandTotal, Status, CreatedAt)
                VALUES
                    (@UserID, @OutletID, @SubTotal, @Discount, 0, @GrandTotal, 'Active', GETDATE());

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

        // Mirrors the product into Products on first sale (ProductCode = ProNumY),
        // then inserts the cart line — so checkout never depends on a separate
        // catalog sync having been run.
        public const string MirrorProductAndInsertCartItem = @"
                    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductCode = @ProNumY)
                        INSERT INTO Products
                            (ProductCode, ProductName, CategoryId, ImageUrl,
                             CostPrice, SellPrice, DiscountPercent, TaxPercent,
                             Status, IsActive, CreatedAt)
                        SELECT
                            p.ProNumY, p.ProName, c.Id, p.ProImage,
                            ISNULL(p.ProImpPri, 0), ISNULL(p.ProUPrSE, 0), 0, 0,
                            1, 1, GETDATE()
                        FROM TPRProducts p
                        LEFT JOIN Category c
                            ON (c.CategoryCode = p.ProCat OR c.Id = TRY_CAST(p.ProCat AS INT))
                        WHERE p.ProNumY = @ProNumY;

                    INSERT INTO CartItems
                        (CartID, ProductID, Quantity, UnitPrice,
                         DiscountPercent, DiscountAmount, TaxPercent, TaxAmount,
                         SubTotal, TotalPrice, Note, CreatedAt)
                    SELECT
                        @CartID, p.ProductID, @Quantity, @UnitPrice,
                        0, 0, 0, 0,
                        @LineTotal, @LineTotal, @Note, GETDATE()
                    FROM Products p
                    WHERE p.ProductCode = @ProNumY;";

        // Stored procedure name reused for both PosCheckout and CheckoutAsync.
        public const string CheckoutProc = "CheckoutCart";

        public const string MarkPaidAfterCheckout = @"
                UPDATE Orders
                SET OrderStatus   = 'Paid',
                    CustomerId    = @CustomerId,
                    PaymentMethod = @PaymentMethod,
                    ShiftId       = @ShiftId,
                    UpdatedAt     = GETDATE()
                WHERE OrderID = @OrderID;";

        public const string GetCustomerPointsById = "SELECT Points FROM Customer WHERE CustomerID = @Id;";

        public const string DeductOutletStock = @"
                    IF NOT EXISTS (
                        SELECT 1 FROM OutletStock
                        WHERE ProNumY = @ProNumY AND OutletId = @OutletId)
                        INSERT INTO OutletStock (OutletId, ProNumY, StockQty, CreatedAt)
                        VALUES (@OutletId, @ProNumY, 0, SYSDATETIME());

                    UPDATE OutletStock
                    SET StockQty = StockQty - @Quantity,
                        UpdatedAt = SYSDATETIME()
                    WHERE ProNumY = @ProNumY AND OutletId = @OutletId;";

        public const string UpdateStatus = @"
                UPDATE Orders
                SET
                    OrderStatus = @Status,
                    UpdatedAt = GETDATE()
                WHERE OrderID = @OrderID;
            ";

        // --- Void helpers ---
        public const string GetOrderRow = "SELECT * FROM Orders WHERE OrderID = @OrderID;";

        public const string MarkVoided = @"
                UPDATE Orders
                SET OrderStatus = 'Cancelled',
                    VoidReason  = @Reason,
                    UpdatedAt   = GETDATE()
                WHERE OrderID = @OrderID;";

        public const string ReturnStockOnVoid = @"
                UPDATE os
                SET os.StockQty = os.StockQty + oi.Quantity,
                    os.UpdatedAt = SYSDATETIME()
                FROM OrderItems oi
                JOIN Products p ON p.ProductID = oi.ProductID
                JOIN OutletStock os
                    ON os.ProNumY = p.ProductCode AND os.OutletId = @OutletId
                WHERE oi.OrderID = @OrderID;";

        public const string GetPointMovesForOrder = @"
                SELECT CustomerId, Points
                FROM PointTransactions
                WHERE OrderId = @OrderID AND TxType IN ('Earn', 'Redeem');";

        public const string ApplyPoints = @"
                UPDATE Customer
                SET Points = Points + @Delta
                WHERE CustomerID = @CustomerId;

                INSERT INTO PointTransactions
                    (CustomerId, OrderId, OutletId, Points, TxType, BalanceAfter, Remark, CreatedAt)
                SELECT
                    @CustomerId, @OrderId, @OutletId, @Delta, @TxType, Points, @Remark, GETDATE()
                FROM Customer
                WHERE CustomerID = @CustomerId;";
    }
}
