namespace JuJuBiAPI.Queries
{
    public static class ShelfLifeQueries
    {
        public const string GetAll = @"
                SELECT
                    ShelfLifeId AS Id,
                    ShelfLifeName,
                    IsActive,
                    ShelfLifeValue,
                    ShelfLifeUnit
                FROM [DBJuJuBi].[dbo].[ShelfLife]
                ORDER BY ShelfLifeId DESC;
            ";

        public const string GetById = @"
                SELECT
                    ShelfLifeId AS Id,
                    ShelfLifeName,
                    IsActive,
                    ShelfLifeValue,
                    ShelfLifeUnit
                FROM [DBJuJuBi].[dbo].[ShelfLife]
                WHERE ShelfLifeId = @Id;
            ";

        public const string Add = @"
                INSERT INTO [DBJuJuBi].[dbo].[ShelfLife]
                (
                    ShelfLifeName,
                    IsActive,
                    ShelfLifeValue,
                    ShelfLifeUnit
                )
                VALUES
                (
                    @ShelfLifeName,
                    @IsActive,
                    @ShelfLifeValue,
                    @ShelfLifeUnit
                );

                SELECT
                    ShelfLifeId AS Id,
                    ShelfLifeName,
                    IsActive,
                    ShelfLifeValue,
                    ShelfLifeUnit
                FROM [DBJuJuBi].[dbo].[ShelfLife]
                WHERE ShelfLifeId = CAST(SCOPE_IDENTITY() AS INT);
            ";

        public const string Update = @"
                UPDATE [DBJuJuBi].[dbo].[ShelfLife]
                SET
                    ShelfLifeName = @ShelfLifeName,
                    IsActive = @IsActive,
                    ShelfLifeValue = @ShelfLifeValue,
                    ShelfLifeUnit = @ShelfLifeUnit
                WHERE ShelfLifeId = @Id;
            ";

        public const string Delete = "DELETE FROM [DBJuJuBi].[dbo].[ShelfLife] WHERE ShelfLifeId = @Id;";
    }
}
