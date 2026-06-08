using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Outlet;
using Dapper;
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
                    OutletCode, OutletName, Province, ProvinceId, Phone, Manager, 
                    Address, Email, Latitude, Longitude, HeadOffice, PhotoPath, 
                    VATNumber, CreatedBy, CreatedAt, IsActive,FranchiseId,Position, GrandOpeningDate
                )
                OUTPUT INSERTED.*
                VALUES
                (
                    @OutletCode, @OutletName, @Province, @ProvinceId, @Phone, @Manager, 
                    @Address, @Email, @Latitude, @Longitude, @HeadOffice, @PhotoPath, 
                    @VATNumber, @CreatedBy, GETDATE(), 1, @FranchiseId, @Position, @GrandOpeningDate
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
           
                var sql = @"UPDATE [dbo].[Outlet]
                    SET OutletCode = @OutletCode,
                        OutletName = @OutletName,
                        Province = @Province,
                        ProvinceId = @ProvinceId,
                        Phone = @Phone,
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
                        GrandOpeningDate = @GrandOpeningDate
                    WHERE Id = @Id";

                var result = await _dbConnection.ExecuteAsync(sql, outletDto, transaction);

               
                if (outletDto.PhotoPaths != null)
                {
              
                    var sqlDeletePhotos = "DELETE FROM [dbo].[OutletPhoto] WHERE OutletId = @Id";
                    await _dbConnection.ExecuteAsync(sqlDeletePhotos, new { Id = outletDto.Id }, transaction);

               
                    if (outletDto.PhotoPaths.Any())
                    {
                        var sqlInsertPhoto = "INSERT INTO [dbo].[OutletPhoto] (OutletId, PhotoPath) VALUES (@OutletId, @PhotoPath)";
                        var photoRecords = outletDto.PhotoPaths.Select(path => new { OutletId = outletDto.Id, PhotoPath = path });
                        await _dbConnection.ExecuteAsync(sqlInsertPhoto, photoRecords, transaction);
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
            if (_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();

   
            using var transaction = _dbConnection.BeginTransaction();

            try
            {

                var sqlUpdateOutlet = "UPDATE [dbo].[Outlet] SET [IsActive] = 0, [UpdatedAt] = GETDATE() WHERE Id = @Id";

                var result = await _dbConnection.ExecuteAsync(sqlUpdateOutlet, new { Id = id }, transaction);

                transaction.Commit();

                return result > 0;
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
    f.outletName as OutletName,
	fty.TypeName,
    o.Province,
    o.Phone,
    o.Manager,
    o.HeadOffice,
    o.PhotoPath,
    o.VATNumber,
    o.ProvinceId,
    o.IsActive,
    o.FranchiseId,
    o.Position,
    o.GrandOpeningDate,
    
    p.PhotoPath AS Url -- ប្តូរឈ្មោះ Alias ឱ្យត្រូវនឹង Property 'Url' នៅក្នុង Photo Class
FROM DBJuJuBi.[dbo].[Outlet] o
LEFT JOIN DBJuJuBi.[dbo].[OutletPhoto] p ON o.Id = p.OutletId
left join DBJuJuBi.[dbo].franchise f on f.franchiseId = o.FranchiseId 
inner join DBJuJuBi.dbo.franchise_type  fty on fty.Id = f.FranchiseTypeId
WHERE o.IsActive = 1
ORDER BY o.Id DESC";

            var outletDict = new Dictionary<int, OutletListDto>();

   
            var result = await _dbConnection.QueryAsync<OutletListDto, Photo, OutletListDto>(
                sql,
                (outlet, photo) =>
                {
                    if (!outletDict.TryGetValue(outlet.Id, out var current))
                    {
                        current = outlet;
                        current.Photos = new List<Photo>();
                        outletDict.Add(current.Id, current);
                    }

               
                    if (photo != null && !string.IsNullOrEmpty(photo.Url))
                    {
                        current.Photos.Add(photo);
                    }

                    return current;
                },
                splitOn: "Url"
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