namespace JuJuBiAPI.Queries
{
    public static class TransferOrderQueries
    {
        public const string OutletExists = "SELECT COUNT(1) FROM [dbo].[Outlet] WHERE Id = @OutletId;";

        public const string InsertHeader = @"
                    INSERT INTO TransferOrders
                    (
                        FromOutletId,
                        ToOutletId,
                        TransferDate,
                        Status,
                        Remark,
                        CreatedBy,
                        CreatedAt
                    )
                    VALUES
                    (
                        @FromOutletId,
                        @ToOutletId,
                        GETDATE(),
                        'Pending',
                        @Remark,
                        @CreatedBy,
                        GETDATE()
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                ";

        public const string InsertItem = @"
                    INSERT INTO TransferOrderItems
                    (
                        TransferOrderId,
                        ProNumY,
                        Qty,
                        ReceivedQty,
                        UnitCost,
                        Remark
                    )
                    VALUES
                    (
                        @TransferOrderId,
                        @ProNumY,
                        @Qty,
                        0,
                        @UnitCost,
                        @Remark
                    );
                ";

        public const string SetTransferNo = @"
                    UPDATE TransferOrders
                    SET TransferNo = @TransferNo
                    WHERE TransferOrderId = @TransferOrderId;
                ";

        public const string GetById = @"
                SELECT * FROM TransferOrders
                WHERE TransferOrderId = @TransferOrderId;

                SELECT * FROM TransferOrderItems
                WHERE TransferOrderId = @TransferOrderId;
            ";

        public const string GetAll = @"
                SELECT * FROM TransferOrders
                ORDER BY CreatedAt DESC;
            ";

        public const string GetByOutlet = @"
                SELECT * FROM TransferOrders
                WHERE FromOutletId = @OutletId OR ToOutletId = @OutletId
                ORDER BY CreatedAt DESC;
            ";

        public const string UpdateStatus = @"
                UPDATE TransferOrders
                SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE TransferOrderId = @TransferOrderId;
            ";

        // --- Approve (ship): deduct from source outlet stock ---
        public const string GetSourceStock = @"
                        SELECT StockQty FROM OutletStock
                        WHERE ProNumY = @ProNumY AND OutletId = @OutletId;
                    ";

        public const string DeductSourceStock = @"
                        UPDATE OutletStock
                        SET StockQty = StockQty - @Qty, UpdatedAt = GETDATE()
                        WHERE ProNumY = @ProNumY AND OutletId = @OutletId;
                    ";

        public const string SetApproved = @"
                    UPDATE TransferOrders
                    SET Status = 'Approved',
                        ApprovedBy = @ApprovedBy,
                        ApprovedAt = GETDATE(),
                        UpdatedAt = GETDATE()
                    WHERE TransferOrderId = @TransferOrderId;
                ";

        // --- Receive: add to destination outlet stock ---
        public const string DestHasStockRow = @"
                        SELECT CASE WHEN EXISTS (
                            SELECT 1 FROM OutletStock
                            WHERE ProNumY = @ProNumY AND OutletId = @OutletId
                        ) THEN 1 ELSE 0 END;
                    ";

        public const string AddDestStock = @"
                            UPDATE OutletStock
                            SET StockQty = StockQty + @Qty, UpdatedAt = GETDATE()
                            WHERE ProNumY = @ProNumY AND OutletId = @OutletId;
                        ";

        public const string InsertDestStock = @"
                            INSERT INTO OutletStock
                                (OutletId, ProNumY, StockQty, CreatedAt)
                            VALUES
                                (@OutletId, @ProNumY, @StockQty, GETDATE());
                        ";

        public const string IncrementReceivedQty = @"
                        UPDATE TransferOrderItems
                        SET ReceivedQty = ReceivedQty + @ReceivedQty
                        WHERE TransferOrderItemId = @TransferOrderItemId;
                    ";

        public const string SetReceived = @"
                    UPDATE TransferOrders
                    SET Status = @Status,
                        ReceivedBy = @ReceivedBy,
                        ReceivedAt = GETDATE(),
                        UpdatedAt = GETDATE()
                    WHERE TransferOrderId = @TransferOrderId;
                ";

        public const string Delete = @"
                DELETE FROM TransferOrderItems WHERE TransferOrderId = @TransferOrderId;
                DELETE FROM TransferOrders WHERE TransferOrderId = @TransferOrderId;
            ";
    }
}
