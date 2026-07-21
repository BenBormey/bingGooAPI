namespace JuJuBiAPI.Queries
{
    public static class CartQueries
    {
        public const string GetActiveCartByUser = @"
                SELECT * FROM Carts
                WHERE UserID = @UserID
                  AND Status = 'Active'";

        public const string CreateCart = @"
                INSERT INTO Carts
                (
                    UserID, OutletID,
                    Status, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES
                (
                    @UserID, @OutletID,
                    @Status, GETDATE()
                )";

        public const string GetCartItem = @"
                SELECT * FROM CartItems
                WHERE CartID = @CartID
                  AND ProductID = @ProductID";

        public const string AddCartItem = @"
                INSERT INTO CartItems
                (
                    CartID, ProductID,
                    Quantity, UnitPrice,
                    DiscountPercent, DiscountAmount,
                    TaxPercent, TaxAmount,
                    SubTotal, TotalPrice,
                    CreatedAt
                )
                VALUES
                (
                    @CartID, @ProductID,
                    @Quantity, @UnitPrice,
                    @DiscountPercent, @DiscountAmount,
                    @TaxPercent, @TaxAmount,
                    @SubTotal, @TotalPrice,
                    GETDATE()
                )";

        public const string UpdateCartItem = @"
                UPDATE CartItems
                SET
                    Quantity = @Quantity,
                    UnitPrice = @UnitPrice,
                    DiscountPercent = @DiscountPercent,
                    DiscountAmount = @DiscountAmount,
                    TaxPercent = @TaxPercent,
                    TaxAmount = @TaxAmount,
                    SubTotal = @SubTotal,
                    TotalPrice = @TotalPrice
                WHERE CartItemID = @CartItemID";

        public const string GetCartIdByItem = "SELECT CartID FROM CartItems WHERE CartItemID=@Id";

        public const string RemoveCartItem = @"DELETE FROM CartItems WHERE CartItemID=@CartItemID";

        public const string UpdateCart = @"
                UPDATE Carts
                SET
                    SubTotal = @SubTotal,
                    DiscountAmount = @DiscountAmount,
                    TaxAmount = @TaxAmount,
                    GrandTotal = @GrandTotal,
                    UpdatedAt = GETDATE()
                WHERE CartID = @CartID";

        public const string LoadItems = @"SELECT * FROM CartItems WHERE CartID=@CartID";

        public const string CartTotals = @"
                SELECT
    ISNULL(SUM(SubTotal), 0) AS SubTotal,
    ISNULL(SUM(DiscountAmount), 0) AS DiscountAmount,
    ISNULL(SUM(TaxAmount), 0) AS TaxAmount,
    ISNULL(SUM(TotalPrice), 0) AS GrandTotal
    FROM CartItems
    WHERE CartID = @CartID;
";
    }
}
