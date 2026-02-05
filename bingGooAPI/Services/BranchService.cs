using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Branch;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
{
    public class BranchService : IbrandRepository
    {
        private readonly IDbConnection _connection;

        public BranchService(IDbConnection connection)
        {
            _connection = connection;
        }

  
        public async Task<Branch> CreateAsync(CreateBranch model)
        {
            var sql = @"
                INSERT INTO Branch
                    (BranchCode, BranchName, Active, CreatedAt)
                VALUES
                    (@BranchCode, @BranchName, @Active, GETDATE());

                SELECT * FROM Branch
                WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _connection.QuerySingleAsync<Branch>(sql, model);
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"
                DELETE FROM Branch
                WHERE Id = @Id
            ";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });

            return rows > 0;
        }

        // GET ALL
        public async Task<IEnumerable<Branch>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    Id,
                    BranchCode,
                    BranchName,
                    Active,
                    CreatedAt
                FROM Branch
                ORDER BY Id DESC
            ";

            return await _connection.QueryAsync<Branch>(sql);
        }

        // GET BY ID
        public async Task<Branch?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT
                    Id,
                    BranchCode,
                    BranchName,
                    Active,
                    CreatedAt
                FROM Branch
                WHERE Id = @Id
            ";

            return await _connection.QueryFirstOrDefaultAsync<Branch>(
                sql,
                new { Id = id }
            );
        }

        // UPDATE
        public async Task<bool> UpdateAsync(Branch model)
        {
            var sql = @"
                UPDATE Branch
                SET
                    BranchCode = @BranchCode,
                    BranchName = @BranchName,
                    Active = @Active
                WHERE Id = @Id
            ";

            var rows = await _connection.ExecuteAsync(sql, model);

            return rows > 0;
        }
    }
}
