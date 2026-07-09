using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Outlet;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace bingGooAPI.Services
{
    public class OutletRepository : IOutletRepository
    {
        private readonly IDbConnection _dbConnection;

        public OutletRepository(IDbConnection context)
        {
            _dbConnection = context;
        }

        public async Task<Outlet> AddAsync(CreateOutletDtos outletDto)
        {
            var sqlOutlet = @"
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

            if (_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                var newOutlet = await _dbConnection.QuerySingleAsync<Outlet>(sqlOutlet, outletDto, transaction);

                if (outletDto.PhotoPaths != null && outletDto.PhotoPaths.Any())
                {
                    var sqlPhoto = "INSERT INTO [dbo].[OutletPhoto] (OutletId, PhotoPath) VALUES (@OutletId, @PhotoPath)";
                    var photoRecords = outletDto.PhotoPaths.Select(path => new { OutletId = newOutlet.Id, PhotoPath = path });

                    await _dbConnection.ExecuteAsync(sqlPhoto, photoRecords, transaction);
                    newOutlet.Images = photoRecords.Select(p => new OutletPhoto { OutletId = p.OutletId, PhotoPath = p.PhotoPath }).ToList();
                }
                if(outletDto.CitizenshipPhotos != null && outletDto.CitizenshipPhotos.Any())
                {
                    var sqlCitizenshipPhoto = "INSERT INTO [DBJuJuBi].[dbo].[CitizenshipPhotos] (OutletId, ImageUrl) VALUES (@OutletId, @ImageUrl)";
                    var citizenshipPhotoRecords = outletDto.CitizenshipPhotos.Select(path => new { OutletId = newOutlet.Id, ImageUrl = path });
                    await _dbConnection.ExecuteAsync(sqlCitizenshipPhoto, citizenshipPhotoRecords, transaction);
                    newOutlet.CitizenshipPhotos = citizenshipPhotoRecords.Select(p => new CitizenshipPhoto { OutletId = p.OutletId, ImageUrl = p.ImageUrl }).ToList();
                }

                transaction.Commit();
                return newOutlet;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UpdateOutletDto outletDto)
        {
         
            if (_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            try
            {
           
                var sql = @"UPDATE [DBJuJuBi].[dbo].[Outlet]
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

                var result = await _dbConnection.ExecuteAsync(sql, outletDto, transaction);

               
                if (outletDto.PhotoPaths != null)
                {
              
                    var sqlDeletePhotos = "DELETE FROM [DBJuJuBi].[dbo].[OutletPhoto] WHERE OutletId = @Id";
                    await _dbConnection.ExecuteAsync(sqlDeletePhotos, new { Id = outletDto.Id }, transaction);

               
                    if (outletDto.PhotoPaths.Any())
                    {
                        var sqlInsertPhoto = "INSERT INTO [DBJuJuBi].[dbo].[OutletPhoto] (OutletId, PhotoPath) VALUES (@OutletId, @PhotoPath)";
                        var photoRecords = outletDto.PhotoPaths.Select(path => new { OutletId = outletDto.Id, PhotoPath = path });
                        await _dbConnection.ExecuteAsync(sqlInsertPhoto, photoRecords, transaction);
                    }
                }
                if(outletDto.CitizenshipPhotos != null)
                {
                    var sqlDeleteCitizenshipPhotos = "DELETE FROM [DBJuJuBi].[dbo].[CitizenshipPhotos] WHERE OutletId = @Id";
                    await _dbConnection.ExecuteAsync(sqlDeleteCitizenshipPhotos, new { Id = outletDto.Id }, transaction);
                    if (outletDto.CitizenshipPhotos.Any())
                    {
                        var sqlInsertCitizenshipPhoto = "INSERT INTO [DBJuJuBi].[dbo].[CitizenshipPhotos] (OutletId, ImageUrl) VALUES (@OutletId, @ImageUrl)";
                        var citizenshipPhotoRecords = outletDto.CitizenshipPhotos.Select(path => new { OutletId = outletDto.Id, ImageUrl = path });
                        await _dbConnection.ExecuteAsync(sqlInsertCitizenshipPhoto, citizenshipPhotoRecords, transaction);
                    }
                }

                transaction.Commit();
                return result > 0;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                var sql = @"DELETE FROM [DBJuJuBi].[dbo].[Outlet] WHERE Id = @Id";

                var result = await _dbConnection.ExecuteAsync(sql, new { Id = id }, transaction);

                transaction.Commit();
                return result > 0;
            }
            catch (SqlException ex) when (ex.Number == 547) // 547 = foreign key / reference constraint
            {
                transaction.Rollback();
                throw new InvalidOperationException(
                    "Cannot delete this outlet because it is still linked to one or more users. " +
                    "Please remove or reassign those users first.");
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task<IEnumerable<OutletListDto>> GetAllAsync()
        {
            var sql = @"
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

            var outletDict = new Dictionary<int, OutletListDto>();

            await _dbConnection.QueryAsync<
                OutletListDto,
                Photo,
                CitizenshipPhoto,
                OutletListDto>
            (
                sql,
                (outlet, photo, citizenshipPhoto) =>
                {
                    if (!outletDict.TryGetValue(outlet.Id, out var current))
                    {
                        current = outlet;

                        current.Photos = new List<Photo>();
                        current.CitizenshipPhotos = new List<CitizenshipPhoto>();

                        outletDict.Add(current.Id, current);
                    }

          
                    if (photo != null &&
                        !string.IsNullOrWhiteSpace(photo.Url) &&
                        !current.Photos.Any(x => x.Url == photo.Url))
                    {
                        current.Photos.Add(photo);
                    }

      
                    if (citizenshipPhoto != null &&
                        citizenshipPhoto.Id > 0 &&
                        !current.CitizenshipPhotos.Any(x => x.Id == citizenshipPhoto.Id))
                    {
                        current.CitizenshipPhotos.Add(citizenshipPhoto);
                    }

                    return current;
                },
                splitOn: "Url,Id"
            );

            return outletDict.Values;
        }

        public async Task<OutletListDto?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT * FROM [dbo].[Outlet] WHERE Id = @id;
                SELECT [PhotoPath] AS Url FROM [DBiggoUnt].[dbo].[OutletPhoto] WHERE OutletId = @id;";

            using (var multi = await _dbConnection.QueryMultipleAsync(sql, new { id }))
            {
                var outlet = await multi.ReadFirstOrDefaultAsync<OutletListDto>();
                if (outlet != null)
                {
                    // ទាញយកទិន្នន័យជា List<Photo> ដោយផ្ទាល់
                    var photos = await multi.ReadAsync<Photo>();
                    outlet.Photos = photos.Where(p => !string.IsNullOrEmpty(p.Url)).ToList();
                }
                return outlet;
            }
        }

        public async Task<bool> OutletExistsAsync(string outletCode)
        {
            var sql = "SELECT COUNT(1) FROM [dbo].[Outlet] WHERE OutletCode = @outletCode";
            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { outletCode });
            return count > 0;
        }

        public async Task<OutletListDto?> GetByCodeAsync(string outletCode)
        {
            var sql = "SELECT * FROM [dbo].[Outlet] WHERE OutletCode = @outletCode";
            return await _dbConnection.QueryFirstOrDefaultAsync<OutletListDto>(sql, new { outletCode });
        }
    }
}