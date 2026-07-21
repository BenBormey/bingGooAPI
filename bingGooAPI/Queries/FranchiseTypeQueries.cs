namespace JuJuBiAPI.Queries
{
    public static class FranchiseTypeQueries
    {
        public const string Create = @"
                INSERT INTO franchise_type
                    (TypeName, Description, IsActive, CreatedDate)
                VALUES
                    (@TypeName, @Description, @IsActive, GETDATE());

                SELECT Id, TypeName, Description, IsActive, CreatedDate
                FROM franchise_type
                WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            ";

        public const string Delete = @"
                DELETE FROM franchise_type
                WHERE Id = @Id
            ";

        public const string GetAll = @"
                SELECT
                    Id,
                    TypeName,
                    Description,
                    IsActive,
                    CreatedDate
                FROM franchise_type
                ORDER BY Id DESC
            ";

        public const string GetById = @"
                SELECT
                    Id,
                    TypeName,
                    Description,
                    IsActive,
                    CreatedDate
                FROM franchise_type
                WHERE Id = @Id
            ";

        public const string Update = @"
                UPDATE franchise_type
                SET
                    TypeName = @TypeName,
                    Description = @Description,
                    IsActive = @IsActive
                WHERE Id = @Id
            ";
    }
}
