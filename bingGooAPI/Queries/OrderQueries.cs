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

        // Column name stays ProductID (table/schema unchanged); the value
        // stored in it is now the TPRProducts.ProNumY (Products table is gone).
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
                    @ProNumY,
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

        // OrderItems.ProductID column name kept as-is; it now holds the
        // TPRProducts.ProNumY value, so we join on that instead of Products.
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
                    TP.ProName   AS ProductName,
                    TP.ProNumY   AS ProductCode,
                    TP.ProImage  AS ImageUrl
                FROM OrderItems OI
                    LEFT JOIN TPRProducts TP ON TP.ProNumY = OI.ProductID
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

        // The single outlet-wide VAT percentage (0 if not configured yet).
        public const string GetVatPercent =
            "SELECT ISNULL((SELECT [Percent] FROM VatSetting WHERE Id = 1), 0);";

        public const string InsertCart = @"
                INSERT INTO Carts
                    (UserID, OutletID, SubTotal, DiscountAmount, TaxAmount, GrandTotal, Status, CreatedAt)
                VALUES
                    (@UserID, @OutletID, @SubTotal, @Discount, @TaxAmount, @GrandTotal, 'Active', GETDATE());

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

        // Products table is gone, so there is nothing left to "mirror".
        // We just confirm the product still exists in TPRProducts, then
        // insert the cart line keyed directly on ProNumY.
        public const string MirrorProductAndInsertCartItem = @"
                    IF NOT EXISTS (SELECT 1 FROM TPRProducts WHERE ProNumY = @ProNumY)
                        THROW 50001, 'Product not found in TPRProducts', 1;

                    INSERT INTO CartItems
                        (CartID, ProductID, Quantity, UnitPrice,
                         DiscountPercent, DiscountAmount, TaxPercent, TaxAmount,
                         SubTotal, TotalPrice, Note, CreatedAt)
                    VALUES
                        (@CartID, @ProNumY, @Quantity, @UnitPrice,
                         0, 0, 0, 0,
                         @LineTotal, @LineTotal, @Note, GETDATE());";

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

        // OrderItems.ProductID column name kept as-is; value = ProNumY.
        // No more join through Products, matched straight to OutletStock.
        public const string ReturnStockOnVoid = @"
                UPDATE os
                SET os.StockQty = os.StockQty + oi.Quantity,
                    os.UpdatedAt = SYSDATETIME()
                FROM OrderItems oi
                JOIN OutletStock os
                    ON os.ProNumY = oi.ProductID AND os.OutletId = @OutletId
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