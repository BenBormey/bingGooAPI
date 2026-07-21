namespace JuJuBiAPI.Queries
{
    public static class PaymentQueries
    {
        public const string Create = @"
                INSERT INTO Payments
                (
                    OrderID,
                    PaymentMethod,
                    AmountPaid,
                    CashReceived,
                    PaymentStatus,
                    TransactionNo,
                    PaidAt,
                    CreatedAt
                )
                VALUES
                (
                    @OrderID,
                    @PaymentMethod,
                    @AmountPaid,
                    @CashReceived,
                    @PaymentStatus,
                    @TransactionNo,
                    GETDATE(),
                    GETDATE()
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

        public const string GetByOrderId = @"
                SELECT TOP 1 *
                FROM Payments
                WHERE OrderID = @OrderID
                ORDER BY CreatedAt DESC;
            ";

        public const string UpdateStatus = @"
                UPDATE Payments
                SET
                    PaymentStatus = @Status,
                    TransactionNo = ISNULL(@TransactionNo, TransactionNo)
                WHERE PaymentID = @PaymentID;
            ";

        public const string GetByDate = @"
                SELECT
                    PaymentID,
                    OrderID,
                    PaymentMethod,
                    AmountPaid,
                    CashReceived,
                    PaymentStatus,
                    TransactionNo,
                    PaidAt,
                    CreatedAt
                FROM Payments
                WHERE PaidAt BETWEEN @From AND @To
                ORDER BY PaidAt DESC;
            ";
    }
}
