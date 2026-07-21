namespace JuJuBiAPI.Queries
{
    public static class ProvinceQueries
    {
        public const string GetAll = @"
                SELECT
                    ProvinceId,
                    ProvinceNameKH,
                    ProvinceNameEN,
                    Code
                FROM Provinces
                ORDER BY ProvinceId ASC";

        public const string GetById = @"
                SELECT
                    ProvinceId,
                    ProvinceNameKH,
                    ProvinceNameEN,
                    Code
                FROM Provinces
                WHERE ProvinceId = @Id";

        public const string Create = @"
                INSERT INTO Provinces (ProvinceNameKH, ProvinceNameEN, Code)
                VALUES (@ProvinceNameKH, @ProvinceNameEN, @Code);

                SELECT * FROM Provinces
                WHERE ProvinceId = CAST(SCOPE_IDENTITY() AS INT);";

        public const string Update = @"
                UPDATE Provinces
                SET
                    ProvinceNameKH = @ProvinceNameKH,
                    ProvinceNameEN = @ProvinceNameEN,
                    Code = @Code
                WHERE ProvinceId = @ProvinceId";

        public const string Delete = "DELETE FROM Provinces WHERE ProvinceId = @Id";
    }
}
