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

        // --- កូដ ADD រក្សាទុកដូចដើម (Don't change) ---
        public async Task<Outlet> AddAsync(CreateOutletDtos outletDto)
        {
            var sqlOutlet = @"
                INSERT INTO [dbo].[Outlet]
                (
                    OutletCode, OutletName, Province, ProvinceId, Phone, Manager, 
                    Address, Email, Latitude, Longitude, HeadOffice, PhotoPath, 
                    VATNumber, CreatedBy, CreatedAt, IsActive
                )
                OUTPUT INSERTED.*
                VALUES
                (
                    @OutletCode, @OutletName, @Province, @ProvinceId, @Phone, @Manager, 
                    @Address, @Email, @Latitude, @Longitude, @HeadOffice, @PhotoPath, 
                    @VATNumber, @CreatedBy, GETDATE(), 1
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

        // --- កូដ UPDATE (Update ទាំង Table Outlet និង PhotoPath) ---
        public async Task<bool> UpdateAsync(UpdateOutletDto outletDto)
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
                            PhotoPath = @PhotoPath, -- Update រូបភាពមេ
                            VATNumber = @VATNumber,
                            UpdatedAt = GETDATE()
                       
                        WHERE Id = @Id";

            var result = await _dbConnection.ExecuteAsync(sql, outletDto);
            return result > 0;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            if (_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            try
            {
          
                var sqlDeletePhotos = "DELETE FROM [dbo].[OutletPhoto] WHERE OutletId = @Id";
                await _dbConnection.ExecuteAsync(sqlDeletePhotos, new { Id = id }, transaction);

           
                var sqlDeleteOutlet = "DELETE FROM [dbo].[Outlet] WHERE Id = @Id";
                var result = await _dbConnection.ExecuteAsync(sqlDeleteOutlet, new { Id = id }, transaction);

                transaction.Commit();
                return result > 0;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        // --- កូដ GET ALL (ប្រាកដថាទាញ PhotoPath មកបង្ហាញ) ---
        public async Task<IEnumerable<OutletListDto>> GetAllAsync()
        {
           
            var sql = @"SELECT 
                            Id, OutletCode, OutletName, Province, Phone, Manager, 
                            HeadOffice, PhotoPath, VATNumber, ProvinceId, IsActive 
                        FROM [dbo].[Outlet] 
                        WHERE IsActive = 1 
                        ORDER BY Id DESC";

            return await _dbConnection.QueryAsync<OutletListDto>(sql);
        }

        public async Task<OutletListDto?> GetByIdAsync(int id)
        {
            var sql = @"
        SELECT * FROM [dbo].[Outlet] WHERE Id = @id;
        SELECT [PhotoPath] FROM [DBiggoUnt].[dbo].[OutletPhoto] WHERE OutletId = @id;";

            using (var multi = await _dbConnection.QueryMultipleAsync(sql, new { id }))
            {
                var outlet = await multi.ReadFirstOrDefaultAsync<OutletListDto>();
                if (outlet != null)
                {
             
                    var photos = await multi.ReadAsync<string>();

                    outlet.Photos = photos.ToList();
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