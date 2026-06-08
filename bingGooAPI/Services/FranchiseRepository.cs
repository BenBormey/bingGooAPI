using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
{
    public class FranchiseRepository : IFranchiseRepository
    {
        private readonly IDbConnection _connection;

        public FranchiseRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Franchise>> GetAllAsync()
        {
            var sql = @"
         SELECT 
     f.FranchiseId,
     f.Outlet,
     f.OutletName,
     f.FranchiseInformation,
     f.FranchiseTypeId,
	 ft.TypeName
 FROM Franchise f inner join franchise_type ft
 on ft.Id  = f.FranchiseTypeId
 ORDER BY f.FranchiseId DESC
;

            ";

            var result = await _connection.QueryAsync<Franchise>(sql);
            return result.AsList();
        }

        public async Task<Franchise> GetByIdAsync(int id)
        {
            var sql = @"
 SELECT 
     f.FranchiseId,
     f.Outlet,
     f.OutletName,
     f.FranchiseInformation,
     f.FranchiseTypeId,
	 ft.TypeName
 FROM Franchise f inner join franchise_type ft
 on ft.Id  = f.FranchiseTypeId
 WHERE FranchiseId = @Id
 ORDER BY f.FranchiseId DESC
;


                

          

            ";

            return await _connection.QueryFirstOrDefaultAsync<Franchise>(
                sql,
                new { Id = id }
            );
        }

        public async Task<int> InsertAsync(Franchise franchise)
        {
            var sql = @"
                INSERT INTO Franchise
                    (Outlet, OutletName, FranchiseInformation,FranchiseTypeId)
                VALUES
                    (@Outlet, @OutletName, @FranchiseInformation,@FranchiseTypeId)
            ";

            return await _connection.ExecuteAsync(sql, franchise);
        }

        public async Task<int> UpdateAsync(Franchise franchise)
        {
            var sql = @"
                UPDATE Franchise
                SET 
                    Outlet = @Outlet,
                    OutletName = @OutletName,
                    FranchiseInformation = @FranchiseInformation,
                    FranchiseTypeId = @FranchiseTypeId
                WHERE FranchiseId = @FranchiseId
            ";

            return await _connection.ExecuteAsync(sql, franchise);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = @"
                DELETE FROM Franchise
                WHERE FranchiseId = @Id
            ";

            return await _connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}