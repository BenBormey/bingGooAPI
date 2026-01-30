using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Branch;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace bingGooAPI.Services
{
    public class BranchService : Ibrand
    {
        private readonly IDbConnection _connection;

        public BranchService(IDbConnection connection)
        {
            _connection = connection;
        }

        // ================= GET ALL =================
        public async Task<IEnumerable<Branch>> GetAllAsync()
        {
            var sql = @"
SELECT 
    Id,
    BranchCode,
    BranchName,
    Active,
    CreatedAt
FROM Branch";

            return await _connection.QueryAsync<Branch>(sql);
        }

        // ================= GET BY ID =================
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
WHERE Id = @Id";

            return await _connection.QueryFirstOrDefaultAsync<Branch>(
                sql,
                new { Id = id }
            );
        }

        // ================= CREATE =================
        public async Task<Branch> CreateAsync(CreateBranch model)
        {
            var sql = @"
INSERT INTO Branch
(
    BranchCode,
    BranchName,
    Active,
Remark,
    CreatedAt
)
VALUES
(
    @BranchCode,
    @BranchName,
    @Active,@Remark,
    GETDATE()
);

SELECT CAST(SCOPE_IDENTITY() AS INT);
";

            // Insert + Get ID
            var id = await _connection.ExecuteScalarAsync<int>(sql, model);

            // Return new entity
            return new Branch
            {
                Id = id,
                BranchCode = model.BranchCode,
                BranchName = model.BranchName,
                Active = model.Active,
                CreatedAt = System.DateTime.Now
            };
        }

        // ================= UPDATE =================
        public async Task<bool> UpdateAsync(Branch model)
        {
            var sql = @"
UPDATE Branch SET
    BranchCode = @BranchCode,
    BranchName = @BranchName,
    Remark   = @Remark,
    Active = @Active
WHERE Id = @Id";

            var affected = await _connection.ExecuteAsync(sql, model);

            return affected > 0;
        }

        // ================= DELETE =================
        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"DELETE FROM Branch WHERE Id = @Id";

            var affected = await _connection.ExecuteAsync(
                sql,
                new { Id = id }
            );

            return affected > 0;
        }
    }
}
