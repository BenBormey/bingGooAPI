namespace JuJuBiAPI.Queries
{
    public static class CustomerQueries
    {
        public const string GetAll = @"
                SELECT
                    CustomerID,
                    CustomerCode,
                    CustomerName,
                    Phone,
                    Email,
                    Address,
                    Points,
                    IsActive,
                    CreatedAt
                FROM Customer
                ORDER BY CustomerCode;";

        // Matches on phone, name or code so a cashier can type whichever the
        // customer gives them. Active members only, best matches first.
        public const string Search = @"
                SELECT TOP 20
                    CustomerID,
                    CustomerCode,
                    CustomerName,
                    Phone,
                    Email,
                    Address,
                    Points,
                    IsActive,
                    CreatedAt
                FROM Customer
                WHERE IsActive = 1
                  AND (
                        Phone        LIKE @Like
                     OR CustomerName LIKE @Like
                     OR CustomerCode LIKE @Like
                  )
                ORDER BY
                    CASE WHEN Phone = @Exact THEN 0 ELSE 1 END,
                    CustomerName;";

        public const string GetById = @"
                SELECT
                    CustomerID,
                    CustomerCode,
                    CustomerName,
                    Phone,
                    Email,
                    Address,
                    Points,
                    IsActive,
                    CreatedAt
                FROM Customer
                WHERE CustomerID = @Id;";

        public const string Create = @"
                INSERT INTO Customer
                (
                    CustomerCode,
                    CustomerName,
                    Phone,
                    Email,
                    Address,
                    Points,
                    IsActive
                )
                VALUES
                (
                    @CustomerCode,
                    @CustomerName,
                    @Phone,
                    @Email,
                    @Address,
                    @Points,
                    @IsActive
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

        public const string Update = @"
                UPDATE Customer
                SET
                    CustomerCode = @CustomerCode,
                    CustomerName = @CustomerName,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address,
                    Points = @Points,
                    IsActive = @IsActive
                WHERE CustomerID = @CustomerID;";

        public const string Delete = @"
                DELETE FROM Customer
                WHERE CustomerID = @Id;";

        public const string ExistsByCode = @"
                SELECT COUNT(*)
                FROM Customer
                WHERE UPPER(CustomerCode) = UPPER(@CustomerCode)
                  AND (@ExcludeId IS NULL OR CustomerID <> @ExcludeId);";

        public const string GetNextCode = "SELECT ISNULL(MAX(CustomerID), 0) + 1 FROM Customer;";
    }
}
