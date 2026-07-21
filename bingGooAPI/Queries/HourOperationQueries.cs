namespace JuJuBiAPI.Queries
{
    public static class HourOperationQueries
    {
        public const string Add = @"
                INSERT INTO DBJuJuBi.dbo.HourOperation
                (

                    OpenTime,
                    CloseTime,
                    Is24Hours,
                    Status,
                    CreatedAt
                )
                VALUES
                (

                    @OpenTime,
                    @CloseTime,
                    @Is24Hours,
                    @Status,
                    GETDATE()
                );

                SELECT *
                FROM DBJuJuBi.dbo.HourOperation
                WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            ";

        public const string GetAll = @"
                SELECT
                    Id,
                        OpenTime,
                    CloseTime,
                    Is24Hours,
                    Status,
                    CreatedAt
                FROM DBJuJuBi.dbo.HourOperation
                ORDER BY Id DESC;
            ";

        public const string GetById = @"
                SELECT
                    Id,

                    OpenTime,
                    CloseTime,
                    Is24Hours,
                    Status,
                    CreatedAt
                FROM DBJuJuBi.dbo.HourOperation
                WHERE Id = @Id;
            ";

        public const string Update = @"
                UPDATE DBJuJuBi.dbo.HourOperation
                SET

                    OpenTime = @OpenTime,
                    CloseTime = @CloseTime,
                    Is24Hours = @Is24Hours,
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id;
            ";

        public const string Delete = @"
                DELETE FROM DBJuJuBi.dbo.HourOperation
                WHERE Id = @Id;
            ";
    }
}
