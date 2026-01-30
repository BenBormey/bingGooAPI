using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Outlet;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
{
    public class OutletRepository : IOutletRepository
    {
        private readonly IDbConnection dbConnection;
        public OutletRepository(IDbConnection context)
        {
            this.dbConnection = context;            
        }

        public async Task AddAsync(CreateOutletDtos outlet)
        {
            var sql = @"
    INSERT INTO [dbo].[Outlet]
    (
        OutletCode,
        OutletName,
        Province,
        Phone,
        Manager,
        Address,
        Email,
        Latitude,
        Longitude,
        HeadOffice
        ,
        CreatedBy,
        CreatedAt,
        IsActive
    )
    VALUES
    (
        @OutletCode,
        @OutletName,
        @Province,
        @Phone,
        @Manager,
        @Address,
        @Email,
        @Latitude,
        @Longitude,
        @HeadOffice,
        @CreatedBy,
        GETDATE(),
        1
    );
";
       
            await dbConnection.ExecuteAsync(sql, outlet);
        }

        public Task DeleteAsync(int id)
        {
           var sql = @"DELETE FROM Outlets WHERE Id = @Id";
       
            return dbConnection.ExecuteAsync(sql, new { Id = id });
        }

        public Task<IEnumerable<OutletListDto>> GetAllAsync()
        {
            var sql = @"SELECT [Id], 
      [OutletCode]
      ,[OutletName]
      ,[Province]
      ,[Phone]
      ,[Manager]
      ,[Address]
      ,[Email]
      ,[Latitude]
      ,[Longitude]
      ,[HeadOffice]
      ,[CreatedBy]
      ,[UpdatedBy]
      ,[CreatedAt]
      ,[UpdatedAt]
      ,[IsActive]
  FROM [DBAuthentication].[dbo].[Outlet]
";
    
            return dbConnection.QueryAsync<OutletListDto>(sql);
        }

        public Task<OutletListDto?> GetByCodeAsync(string outletCode)
        {
      var sql = @"SELECT [Id],
      [OutletCode]
      ,[OutletName]
      ,[Province]
      ,[Phone]
      ,[Manager]
      ,[Address]
      ,[Email]
      ,[Latitude]
      ,[Longitude]
      ,[HeadOffice]
      ,[CreatedBy]
      ,[UpdatedBy]
      ,[CreatedAt]
      ,[UpdatedAt]
      ,[IsActive]
  FROM [DBAuthentication].[dbo].[Outlet]
  where OutletCode = @outletCode;
";
    
            return dbConnection.QueryFirstOrDefaultAsync<OutletListDto>(sql, new { outletCode });

        }

        public Task<OutletListDto?> GetByIdAsync(int id)
        {
            var sql = @"SELECT [Id],
      [OutletCode]
      ,[OutletName]
      ,[Province]
      ,[Phone]
      ,[Manager]
      ,[Address]
      ,[Email]
      ,[Latitude]
      ,[Longitude]
      ,[HeadOffice]
      ,[CreatedBy]
      ,[UpdatedBy]
      ,[CreatedAt]
      ,[UpdatedAt]
      ,[IsActive]
  FROM [DBAuthentication].[dbo].[Outlet]
  where Id = @id
";
      
            return dbConnection.QueryFirstOrDefaultAsync<OutletListDto>(sql, new { id });
        }

      

        public Task UpdateAsync(UpdateOutletDto outlet)
        {
            var sql = @"UPDATE Outlets 
                        SET OutletCode = @OutletCode,
                            OutletName = @OutletName,
                            Province = @Province,
                            Phone = @Phone,
                            Manager = @Manager,
                            Address = @Address,
                            Email = @Email,
                            Latitude = @Latitude,
                            Longitude = @Longitude,
                            HeadOffice = @HeadOffice,
                            OutletType = @OutletType,
                            ContactPerson = @ContactPerson,
                            Remark = @Remark
                        WHERE Id = @Id";    
          
            return dbConnection.ExecuteAsync(sql, outlet);
        }
    }
}
