namespace JuJuBiAPI.Queries
{
    public static class UomQueries
    {
        public const string GetAll = @"
                SELECT
                    UOMId,
                    UOMCode,
                    UOMName,
                    IsActive
                FROM UOM
                ORDER BY UOMCode;";

        public const string GetById = @"
                SELECT
                    UOMId,
                    UOMCode,
                    UOMName,
                    IsActive
                FROM UOM
                WHERE UOMId = @Id;";

        public const string Create = @"
                INSERT INTO UOM
                (
                    UOMCode,
                    UOMName,
                    IsActive
                )
                VALUES
                (
                    @UOMCode,
                    @UOMName,
                    @IsActive
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

        public const string Update = @"
                UPDATE UOM
                SET
                    UOMCode = @UOMCode,
                    UOMName = @UOMName,
                    IsActive = @IsActive
                WHERE UOMId = @UOMId;";

        public const string Delete = @"
                DELETE FROM UOM
                WHERE UOMId = @Id;";
    }
}
