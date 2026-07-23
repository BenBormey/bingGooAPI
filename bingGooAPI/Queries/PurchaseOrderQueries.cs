namespace JuJuBiAPI.Queries
{
    public static class PurchaseOrderQueries
    {
        public const string GetWarehouseOutletId = "SELECT TOP 1 Id FROM [dbo].[Outlet] WHERE HeadOffice = 1;";

        public const string InsertHeader = @"
                    INSERT INTO PurchaseOrders
                    (
                        SupplierID,
                        OutletID,
                        OrderDate,
                        ExpectedDate,
                        SubTotal,
                        DiscountAmount,
                        TaxAmount,
                        GrandTotal,
                        Status,
                        Note,
                        CreatedAt
                    )
                    VALUES
                    (
                        @SupplierID,
                        @OutletID,
                        GETDATE(),
                        @ExpectedDate,
                        0, 0, 0, 0,
                        'Draft',
                        @Note,
                        GETDATE()
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                ";

        public const string InsertItem = @"
                    INSERT INTO PurchaseOrderItems
                    (
                        PurchaseOrderID,
                        ProNumY,
                        Quantity,
                        UnitCost,
                        DiscountPercent,
                        DiscountAmount,
                        TaxPercent,
                        TaxAmount,
                        SubTotal,
                        TotalCost,
                        ReceivedQty,
                        CreatedAt
                    )
                    VALUES
                    (
                        @PurchaseOrderID,
                        @ProNumY,
                        @Quantity,
                        @UnitCost,
                        @DiscountPercent,
                        @DiscountAmount,
                        @TaxPercent,
                        @TaxAmount,
                        @SubTotal,
                        @TotalCost,
                        0,
                        GETDATE()
                    );
                ";

        public const string UpdateHeaderTotals = @"
                    UPDATE PurchaseOrders
                    SET
                        PurchaseOrderNo = @PurchaseOrderNo,
                        SubTotal = @SubTotal,
                        DiscountAmount = @DiscountAmount,
                        TaxAmount = @TaxAmount,
                        GrandTotal = @GrandTotal,
                        Status = 'Ordered'
                    WHERE PurchaseOrderID = @PurchaseOrderID;
                ";

        public const string GetById = @"
                SELECT * FROM PurchaseOrders
                WHERE PurchaseOrderID = @PurchaseOrderID;

                SELECT * FROM PurchaseOrderItems
                WHERE PurchaseOrderID = @PurchaseOrderID;
            ";

        public const string GetAll = @"
                SELECT * FROM PurchaseOrders
                ORDER BY CreatedAt DESC;
            ";

        public const string GetItemsByOrderIds = @"
                SELECT * FROM PurchaseOrderItems
                WHERE PurchaseOrderID IN @Ids;
            ";

        public const string GetBySupplier = @"
                SELECT * FROM PurchaseOrders
                WHERE SupplierID = @SupplierID
                ORDER BY CreatedAt DESC;
            ";

        public const string GetByOutlet = @"
                SELECT * FROM PurchaseOrders
                WHERE OutletID = @OutletID
                ORDER BY CreatedAt DESC;
            ";

        public const string UpdateStatus = @"
                UPDATE PurchaseOrders
                SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE PurchaseOrderID = @PurchaseOrderID;
            ";

        public const string IncrementReceivedQty = @"
                        UPDATE PurchaseOrderItems
                        SET ReceivedQty = ReceivedQty + @ReceivedQty
                        WHERE PurchaseOrderItemID = @PurchaseOrderItemID;
                    ";

        // Guard: the product must exist. TPRProducts is the product master.
        public const string GetProIdByProNumY = @"
                        SELECT ProID FROM TPRProducts WHERE ProNumY = @ProNumY;
                    ";

        // Stock now lives in OutletStock, keyed by ProNumY + OutletId
        // (same table the OutletOrder / TransferOrder flows use).
        public const string OutletStockExists = @"
                        SELECT CASE WHEN EXISTS (
                            SELECT 1 FROM OutletStock
                            WHERE ProNumY = @ProNumY AND OutletId = @OutletId
                        ) THEN 1 ELSE 0 END;
                    ";

        public const string AddProductStock = @"
                            UPDATE OutletStock
                            SET
                                StockQty = StockQty + @ReceivedQty,
                                UpdatedAt = GETDATE()
                            WHERE ProNumY = @ProNumY AND OutletId = @OutletId;
                        ";

        public const string InsertProductStock = @"
                            INSERT INTO OutletStock
                                (OutletId, ProNumY, StockQty, CreatedAt)
                            VALUES
                                (@OutletId, @ProNumY, @StockQty, GETDATE());
                        ";

        public const string SetStatus = @"
                    UPDATE PurchaseOrders
                    SET Status = @Status, UpdatedAt = GETDATE()
                    WHERE PurchaseOrderID = @PurchaseOrderID;
                ";

        public const string Delete = @"
                DELETE FROM PurchaseOrderItems WHERE PurchaseOrderID = @PurchaseOrderID;
                DELETE FROM PurchaseOrders WHERE PurchaseOrderID = @PurchaseOrderID;
            ";
    }
}
