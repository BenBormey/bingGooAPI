using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Outlet;
using JuJuBiAPI.Queries;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JuJuBiAPI.Repositories
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
            if (_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                var newOutlet = await _dbConnection.QuerySingleAsync<Outlet>(OutletQueries.InsertOutlet, outletDto, transaction);

                if (outletDto.PhotoPaths != null && outletDto.PhotoPaths.Any())
                {
                    var photoRecords = outletDto.PhotoPaths.Select(path => new { OutletId = newOutlet.Id, PhotoPath = path });

                    await _dbConnection.ExecuteAsync(OutletQueries.InsertPhoto, photoRecords, transaction);
                    newOutlet.Images = photoRecords.Select(p => new OutletPhoto { OutletId = p.OutletId, PhotoPath = p.PhotoPath }).ToList();
                }
                if(outletDto.CitizenshipPhotos != null && outletDto.CitizenshipPhotos.Any())
                {
                    var citizenshipPhotoRecords = outletDto.CitizenshipPhotos.Select(path => new { OutletId = newOutlet.Id, ImageUrl = path });
                    await _dbConnection.ExecuteAsync(OutletQueries.InsertCitizenshipPhoto, citizenshipPhotoRecords, transaction);
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
                var result = await _dbConnection.ExecuteAsync(OutletQueries.Update, outletDto, transaction);


                if (outletDto.PhotoPaths != null)
                {
                    await _dbConnection.ExecuteAsync(OutletQueries.DeletePhotos, new { Id = outletDto.Id }, transaction);


                    if (outletDto.PhotoPaths.Any())
                    {
                        var photoRecords = outletDto.PhotoPaths.Select(path => new { OutletId = outletDto.Id, PhotoPath = path });
                        await _dbConnection.ExecuteAsync(OutletQueries.InsertPhotoUpdate, photoRecords, transaction);
                    }
                }
                if(outletDto.CitizenshipPhotos != null)
                {
                    await _dbConnection.ExecuteAsync(OutletQueries.DeleteCitizenshipPhotos, new { Id = outletDto.Id }, transaction);
                    if (outletDto.CitizenshipPhotos.Any())
                    {
                        var citizenshipPhotoRecords = outletDto.CitizenshipPhotos.Select(path => new { OutletId = outletDto.Id, ImageUrl = path });
                        await _dbConnection.ExecuteAsync(OutletQueries.InsertCitizenshipPhotoUpdate, citizenshipPhotoRecords, transaction);
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
                var result = await _dbConnection.ExecuteAsync(OutletQueries.Delete, new { Id = id }, transaction);

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
            var outletDict = new Dictionary<int, OutletListDto>();

            await _dbConnection.QueryAsync<
                OutletListDto,
                Photo,
                CitizenshipPhoto,
                OutletListDto>
            (
                OutletQueries.GetAll,
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
            using (var multi = await _dbConnection.QueryMultipleAsync(OutletQueries.GetById, new { id }))
            {
                var outlet = await multi.ReadFirstOrDefaultAsync<OutletListDto>();
                if (outlet != null)
                {
                    var photos = await multi.ReadAsync<Photo>();
                    outlet.Photos = photos.Where(p => !string.IsNullOrEmpty(p.Url)).ToList();
                }
                return outlet;
            }
        }

        public async Task<bool> OutletExistsAsync(string outletCode)
        {
            var count = await _dbConnection.ExecuteScalarAsync<int>(OutletQueries.Exists, new { outletCode });
            return count > 0;
        }

        public async Task<OutletListDto?> GetByCodeAsync(string outletCode)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<OutletListDto>(OutletQueries.GetByCode, new { outletCode });
        }
    }
}
