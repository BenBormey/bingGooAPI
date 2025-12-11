using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;

namespace bingGooAPI.Services
{
    public class BranchService : Ibrand
    {
        private readonly IDbConnection _connection;

        public BranchService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Branch>> GetAllAsync()
        {
            var sql = @"SELECT Id, BranchCode, BranchName, Active, CreatedAt FROM Branch";
            return await _connection.QueryAsync<Branch>(sql);
        }

        public async Task<Branch?> GetByIdAsync(int id)
        {
            var sql = @"SELECT Id, BranchCode, BranchName, Active, CreatedAt FROM Branch WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Branch>(sql, new { Id = id });
        }

        public async Task<Branch> CreateAsync(Branch model)
        {
            var sql = @"
INSERT INTO Branch (BranchCode, BranchName, Active, CreatedAt)
VALUES (@BranchCode, @BranchName, @Active, @CreatedAt);
SELECT CAST(SCOPE_IDENTITY() as int);";

            model.CreatedAt = DateTime.Now;

            var id = await _connection.ExecuteScalarAsync<int>(sql, model);
            model.Id = id;
            return model;
        }

        public async Task<bool> UpdateAsync(Branch model)
        {
            var sql = @"
UPDATE Branch SET
    BranchCode = @BranchCode,
    BranchName = @BranchName,
    Active = @Active
WHERE Id = @Id";

            var affected = await _connection.ExecuteAsync(sql, model);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"DELETE FROM Branch WHERE Id = @Id";
            var affected = await _connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }
    }
}
