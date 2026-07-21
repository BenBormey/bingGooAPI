namespace JuJuBiAPI.Queries
{
    public static class ProductScalQueries
    {
        private const string SelectColumns = @"
SELECT
    ps.Id,
    ps.ProId,
    ps.ProNumY,
    ps.CTNPerPallet,
    ps.UOMCode,
    u.UOMName,
    ps.Width,
    ps.Length,
    ps.Height,
    ps.CBMPerCTN,
    ps.NetWeight,
    ps.GrossWeight,
    ps.CreatedDate,
    ps.Status
FROM TblProductsScale ps
LEFT JOIN UOM u
    ON u.UOMCode = ps.UOMCode";

        public const string Exists = @"
SELECT COUNT(1)
FROM TblProductsScale
WHERE Status = 1
AND ProNumY = @ProNumY
AND UOMCode = @UOMCode
AND (@ExcludeId IS NULL OR Id <> @ExcludeId);";

        // Return: new Id if inserted, 0 if duplicate
        public const string Create = @"
IF NOT EXISTS
(
    SELECT 1
    FROM TblProductsScale
    WHERE Status = 1
    AND ProNumY = @ProNumY
    AND UOMCode = @UOMCode
)
BEGIN
    INSERT INTO TblProductsScale
    (
        ProId,
        CTNPerPallet,
        UOMCode,
        Width,
        Length,
        Height,
        CBMPerCTN,
        NetWeight,
        GrossWeight,
        CreatedDate,
        Status,
        ProNumY
    )
    VALUES
    (
        @ProId,
        @CTNPerPallet,
        @UOMCode,
        @Width,
        @Length,
        @Height,
        @CBMPerCTN,
        @NetWeight,
        @GrossWeight,
        GETDATE(),
        1,
        @ProNumY
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
ELSE
BEGIN
    SELECT 0;  -- duplicate
END";

        public const string GetAll = SelectColumns + @"
WHERE ps.Status = 1
ORDER BY ps.ProNumY;";

        public const string GetById = SelectColumns + @"
WHERE ps.Id = @Id
AND ps.Status = 1;";

        public const string GetByProductId = SelectColumns + @"
WHERE ps.ProId = @ProductId
AND ps.Status = 1;";

        public const string SearchByProNumY = SelectColumns + @"
WHERE ps.Status = 1
AND ps.ProNumY LIKE @Search
ORDER BY ps.ProNumY;";

        public const string GetByProNumY = SelectColumns + @"
WHERE ps.Status = 1
AND ps.ProNumY = @ProNumY;";

        public const string Update = @"
UPDATE TblProductsScale
SET
    ProId = @ProId,
    CTNPerPallet = @CTNPerPallet,
    UOMCode = @UOMCode,
    Width = @Width,
    Length = @Length,
    Height = @Height,
    CBMPerCTN = @CBMPerCTN,
    NetWeight = @NetWeight,
    GrossWeight = @GrossWeight,
    Status = @Status,
    ProNumY = @ProNumY
WHERE Id = @Id;";

        // Archives the row into TblProductsScaleDelete before removing it.
        public const string Delete = @"
INSERT INTO TblProductsScaleDelete
(
    ScaleId,
    ProId,
    CTNPerPallet,
    Width,
    Length,
    Height,
    CBMPerCTN,
    NetWeight,
    GrossWeight,
    CreatedDate,
    Status,
    DeletedDate
)
SELECT
    Id,
    ProId,
    CTNPerPallet,
    Width,
    Length,
    Height,
    CBMPerCTN,
    NetWeight,
    GrossWeight,
    CreatedDate,
    Status,
    GETDATE()
FROM TblProductsScale
WHERE Id = @Id;

DELETE FROM TblProductsScale
WHERE Id = @Id;

SELECT @@ROWCOUNT;";
    }
}
