namespace JuJuBiAPI.Queries
{
    public static class OutletCodeQueries
    {
        public const string GetAll = @"
                SELECT Id, OutletCode, IsActive
                FROM [DBJuJuBi].[dbo].[OutletCode]
                ORDER BY OutletCode DESC;
            ";

        public const string GetById = @"
                SELECT Id, OutletCode, IsActive
                FROM [DBJuJuBi].[dbo].[OutletCode]
                WHERE Id = @Id;
            ";

        public const string Add = @"
                INSERT INTO [DBJuJuBi].[dbo].[OutletCode]
                (
                    OutletCode,
                    IsActive
                )
                VALUES
                (
                    @OutletCode,
                    @IsActive
                );

                SELECT Id, OutletCode, IsActive
                FROM [DBJuJuBi].[dbo].[OutletCode]
                WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            ";

        public const string Update = @"
                UPDATE [DBJuJuBi].[dbo].[OutletCode]
                SET OutletCode = @OutletCode,
                    IsActive = @IsActive
                WHERE Id = @Id;
            ";

        public const string Delete = "DELETE FROM [DBJuJuBi].[dbo].[OutletCode] WHERE Id = @Id;";

        public const string Exists = @"
                SELECT COUNT(1)
                FROM [DBJuJuBi].[dbo].[OutletCode]
                WHERE UPPER(OutletCode) = UPPER(@OutletCode)
                AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
            ";

        public const string GetNextCode = "SELECT ISNULL(MAX(Id), 0) + 1 FROM [DBJuJuBi].[dbo].[OutletCode];";
    }
}
