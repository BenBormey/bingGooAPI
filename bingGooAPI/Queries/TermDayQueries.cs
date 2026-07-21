namespace JuJuBiAPI.Queries
{
    public static class TermDayQueries
    {
        public const string GetAll = @"
                SELECT
                    Id,
                    CAST(CountDay AS VARCHAR(10)) + ' Day' AS TermDay,
                    CountDay
                FROM [DBJuJuBi].[dbo].[tblTermDay]
                ORDER BY Id DESC;
            ";

        public const string GetById = @"
                SELECT
                    Id,
                    CAST(CountDay AS VARCHAR(10)) + ' Day' AS TermDay,
                    CountDay
                FROM [DBJuJuBi].[dbo].[tblTermDay]
                WHERE Id = @Id;
            ";

        public const string Add = @"
                INSERT INTO [DBJuJuBi].[dbo].[tblTermDay]
                (
                    CountDay
                )
                VALUES
                (
                    @CountDay
                );

                SELECT
                    Id,
                    CAST(CountDay AS VARCHAR(10)) + ' Day' AS TermDay,
                    CountDay
                FROM [DBJuJuBi].[dbo].[tblTermDay]
                WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            ";

        public const string Update = @"
                UPDATE [DBJuJuBi].[dbo].[tblTermDay]
                SET CountDay = @CountDay
                WHERE Id = @Id;
            ";

        public const string Delete = "DELETE FROM [DBJuJuBi].[dbo].[tblTermDay] WHERE Id = @Id;";
    }
}
