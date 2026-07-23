namespace JuJuBiAPI.Queries
{
    public static class OutletOrderQueries
    {
        public const string IsWarehouse = "SELECT HeadOffice FROM [dbo].[Outlet] WHERE Id = @OutletID;";

        public const string InsertHeader = @"
                    INSERT INTO OutletOrders
                    (
                        OutletID,
                        OrderDate,
                        ExpectedDate,
                        Status,
                        Note,
                        CreatedAt
                    )
                    VALUES
                    (
                        @OutletID,
                        GETDATE(),
                        @ExpectedDate,
                        'Requested',
                        @Note,
                        GETDATE()
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                ";

        public const string InsertItem = @"
                    INSERT INTO OutletOrderItems
                    (
                        OutletOrderID,
                        ProNumY,
                        RequestedQty,
                        FulfilledQty,
                        CreatedAt
                    )
                    VALUES
                    (
                        @OutletOrderID,
                        @ProNumY,
                        @RequestedQty,
                        0,
                        GETDATE()
                    );
                ";

        public const string SetOrderNo = @"
                    UPDATE OutletOrders
                    SET OutletOrderNo = @OutletOrderNo
                    WHERE OutletOrderID = @OutletOrderID;
                ";

        public const string GetById = @"
                SELECT * FROM OutletOrders
                WHERE OutletOrderID = @OutletOrderID;

                SELECT * FROM OutletOrderItems
                WHERE OutletOrderID = @OutletOrderID;
            ";

        public const string GetAll = @"
                SELECT * FROM OutletOrders
                ORDER BY CreatedAt DESC;
            ";

        public const string GetByOutlet = @"
                SELECT * FROM OutletOrders
                WHERE OutletID = @OutletID
                ORDER BY CreatedAt DESC;
            ";

        public const string UpdateStatus = @"
                UPDATE OutletOrders
                SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE OutletOrderID = @OutletOrderID;
            ";

        public const string GetWarehouseOutletId = "SELECT TOP 1 Id FROM [dbo].[Outlet] WHERE HeadOffice = 1;";

        public const string GetWarehouseStock = @"
                        SELECT StockQty FROM OutletStock
                        WHERE ProNumY = @ProNumY AND OutletId = @OutletId;
                    ";

        // Whole warehouse stock list — the approval screen shows what's
        // available next to each requested line.
        public const string GetWarehouseStockAll = @"
                        SELECT os.ProNumY, os.StockQty
                        FROM OutletStock os
                        WHERE os.OutletId = (SELECT TOP 1 Id FROM [dbo].[Outlet] WHERE HeadOffice = 1);
                    ";

        public const string DeductWarehouseStock = @"
                        UPDATE OutletStock
                        SET StockQty = StockQty - @FulfilledQty, UpdatedAt = GETDATE()
                        WHERE ProNumY = @ProNumY AND OutletId = @OutletId;
                    ";

        public const string OutletHasStockRow = @"
                        SELECT CASE WHEN EXISTS (
                            SELECT 1 FROM OutletStock
                            WHERE ProNumY = @ProNumY AND OutletId = @OutletId
                        ) THEN 1 ELSE 0 END;
                    ";

        public const string AddOutletStock = @"
                            UPDATE OutletStock
                            SET StockQty = StockQty + @FulfilledQty, UpdatedAt = GETDATE()
                            WHERE ProNumY = @ProNumY AND OutletId = @OutletId;
                        ";

        public const string InsertOutletStock = @"
                            INSERT INTO OutletStock
                                (OutletId, ProNumY, StockQty, CreatedAt)
                            VALUES
                                (@OutletId, @ProNumY, @StockQty, GETDATE());
                        ";

        public const string IncrementFulfilledQty = @"
                        UPDATE OutletOrderItems
                        SET FulfilledQty = FulfilledQty + @FulfilledQty
                        WHERE OutletOrderItemID = @OutletOrderItemID;
                    ";

        public const string SetStatus = @"
                    UPDATE OutletOrders
                    SET Status = @Status, UpdatedAt = GETDATE()
                    WHERE OutletOrderID = @OutletOrderID;
                ";

        public const string Delete = @"
                DELETE FROM OutletOrderItems WHERE OutletOrderID = @OutletOrderID;
                DELETE FROM OutletOrders WHERE OutletOrderID = @OutletOrderID;
            ";
    }
}
