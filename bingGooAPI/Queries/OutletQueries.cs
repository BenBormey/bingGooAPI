namespace JuJuBiAPI.Queries
{
    public static class OutletQueries
    {
        public const string InsertOutlet = @"
                INSERT INTO [dbo].[Outlet]
                (
                    OutletCode, OutletName, Province, ProvinceId, FrancisePhone, Manager, HourOperationId,
                    Address, Email, Latitude, Longitude, HeadOffice, PhotoPath,
                    VATNumber, CreatedBy, CreatedAt, IsActive,FranchiseId,Position, GrandOpeningDate,OutletPhone
                )
                OUTPUT INSERTED.*
                VALUES
                (
                    @OutletCode, @OutletName, @Province, @ProvinceId, @FrancisePhone, @Manager, @HourOperationId,
                    @Address, @Email, @Latitude, @Longitude, @HeadOffice, @PhotoPath,
                    @VATNumber, @CreatedBy, GETDATE(), 1, @FranchiseId, @Position, @GrandOpeningDate,@OutletPhone
                );";

        public const string InsertPhoto = "INSERT INTO [dbo].[OutletPhoto] (OutletId, PhotoPath) VALUES (@OutletId, @PhotoPath)";

        public const string InsertCitizenshipPhoto = "INSERT INTO [DBJuJuBi].[dbo].[CitizenshipPhotos] (OutletId, ImageUrl) VALUES (@OutletId, @ImageUrl)";

        public const string Update = @"UPDATE [DBJuJuBi].[dbo].[Outlet]
                    SET OutletCode = @OutletCode,
                        OutletName = @OutletName,
                        Province = @Province,
                        ProvinceId = @ProvinceId,
                        FrancisePhone = @FrancisePhone,
                        Manager = @Manager,
                        Address = @Address,
                        Email = @Email,
                        Latitude = @Latitude,
                        Longitude = @Longitude,
                        HeadOffice = @HeadOffice,
                        PhotoPath = @PhotoPath,
                        VATNumber = @VATNumber,
                        UpdatedAt = GETDATE(),
                        IsActive = @IsActive,
                        FranchiseId = @FranchiseId,
                        Position = @Position,
                        HourOperationId = @HourOperationId,
                        GrandOpeningDate = @GrandOpeningDate,
                        CitizenshipPhotos= @CitizenshipPhotos,

                        OutletPhone = @OutletPhone
                    WHERE Id = @Id";

        public const string DeletePhotos = "DELETE FROM [DBJuJuBi].[dbo].[OutletPhoto] WHERE OutletId = @Id";

        public const string InsertPhotoUpdate = "INSERT INTO [DBJuJuBi].[dbo].[OutletPhoto] (OutletId, PhotoPath) VALUES (@OutletId, @PhotoPath)";

        public const string DeleteCitizenshipPhotos = "DELETE FROM [DBJuJuBi].[dbo].[CitizenshipPhotos] WHERE OutletId = @Id";

        public const string InsertCitizenshipPhotoUpdate = "INSERT INTO [DBJuJuBi].[dbo].[CitizenshipPhotos] (OutletId, ImageUrl) VALUES (@OutletId, @ImageUrl)";

        public const string Delete = @"DELETE FROM [DBJuJuBi].[dbo].[Outlet] WHERE Id = @Id";

        public const string GetAll = @"
SELECT
    o.Id,
    o.OutletCode,
    f.OutletName AS OutletName,
    fty.TypeName,
    p.ProvinceNameEN,
	o.Address,
    o.FrancisePhone,
    o.Manager,
    o.HeadOffice,
    o.PhotoPath,
    o.VATNumber,
    o.ProvinceId,
    o.IsActive,
    o.FranchiseId,
    o.Position,
    o.Email
,
    o.GrandOpeningDate,
	h.Id AS HourOperationId,
 CONVERT(VARCHAR, h.OpenTime, 108) + ' - ' + CONVERT(VARCHAR, h.CloseTime, 108) AS HourOperation,
    h.OpenTime,
    h.CloseTime,
    h.Is24Hours,
o.OutletPhone,

    op.PhotoPath AS Url,

    cp.Id,
    cp.OutletId,
    cp.ImageUrl,
    cp.CreatedAt

FROM DBJuJuBi.dbo.Outlet o

LEFT JOIN DBJuJuBi.dbo.HourOperation h
    ON h.Id = o.HourOperationId

LEFT JOIN DBJuJuBi.dbo.OutletPhoto op
    ON op.OutletId = o.Id

LEFT JOIN DBJuJuBi.dbo.CitizenshipPhotos cp
    ON cp.OutletId = o.Id

LEFT JOIN DBJuJuBi.dbo.Franchise f
    ON f.FranchiseId = o.FranchiseId

INNER JOIN DBJuJuBi.dbo.Franchise_Type fty
    ON fty.Id = f.FranchiseTypeId

INNER JOIN DBJuJuBi.dbo.Provinces p
    ON p.ProvinceId = o.ProvinceId


ORDER BY o.Id DESC;";

        public const string GetById = @"
                SELECT * FROM [dbo].[Outlet] WHERE Id = @id;
                SELECT [PhotoPath] AS Url FROM [DBiggoUnt].[dbo].[OutletPhoto] WHERE OutletId = @id;";

        public const string Exists = "SELECT COUNT(1) FROM [dbo].[Outlet] WHERE OutletCode = @outletCode";

        public const string GetByCode = "SELECT * FROM [dbo].[Outlet] WHERE OutletCode = @outletCode";
    }
}
