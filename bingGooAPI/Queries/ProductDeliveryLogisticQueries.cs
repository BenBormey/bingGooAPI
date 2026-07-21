namespace JuJuBiAPI.Queries
{
    public static class ProductDeliveryLogisticQueries
    {
        public const string GetByProduct = @"
                SELECT
                    d.Id,
                    d.ProNumY,
                    d.ProvinceId,
                    p.ProvinceNameEN,
                    d.AdditionalCost,
                    d.CreatedAt
                FROM ProductDeliveryLogistics d
                JOIN Provinces p ON p.ProvinceId = d.ProvinceId
                WHERE d.ProNumY = @ProNumY
                ORDER BY p.ProvinceNameEN;";

        public const string Exists = @"
                SELECT COUNT(*)
                FROM ProductDeliveryLogistics
                WHERE ProNumY = @ProNumY AND ProvinceId = @ProvinceId;";

        public const string Insert = @"
                INSERT INTO ProductDeliveryLogistics
                (
                    ProNumY,
                    ProvinceId,
                    AdditionalCost,
                    CreatedAt
                )
                VALUES
                (
                    @ProNumY,
                    @ProvinceId,
                    @AdditionalCost,
                    GETDATE()
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

        public const string GetById = @"
                SELECT
                    d.Id,
                    d.ProNumY,
                    d.ProvinceId,
                    p.ProvinceNameEN,
                    d.AdditionalCost,
                    d.CreatedAt
                FROM ProductDeliveryLogistics d
                JOIN Provinces p ON p.ProvinceId = d.ProvinceId
                WHERE d.Id = @Id;";

        public const string Delete = "DELETE FROM ProductDeliveryLogistics WHERE Id = @Id;";
    }
}
