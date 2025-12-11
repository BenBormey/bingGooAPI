using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;

namespace bingGooAPI.Services
{
    public class CategoryService : ICategory
    {
        private readonly IDbConnection _connection;

        public CategoryService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var sql = @"SELECT Id, CategoryCode, CategoryName, Active, CreatedAt FROM Category";
            return await _connection.QueryAsync<Category>(sql);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            var sql = @"SELECT Id, CategoryCode, CategoryName, Active, CreatedAt FROM Category WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
        }

        public async Task<Category> CreateAsync(Category model)
        {
            var sql = @"
INSERT INTO Category (CategoryCode, CategoryName, Active, CreatedAt)
VALUES (@CategoryCode, @CategoryName, @Active, @CreatedAt);
SELECT CAST(SCOPE_IDENTITY() as int);";

            model.CreatedAt = DateTime.Now;
            var id = await _connection.ExecuteScalarAsync<int>(sql, model);
            model.Id = id;
            return model;
        }

        public async Task<bool> UpdateAsync(Category model)
        {
            var sql = @"
UPDATE Category SET
    CategoryCode = @CategoryCode,
    CategoryName = @CategoryName,
    Active = @Active
WHERE Id = @Id";

            var affected = await _connection.ExecuteAsync(sql, model);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"DELETE FROM Category WHERE Id = @Id";
            var affected = await _connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }
    }
}
