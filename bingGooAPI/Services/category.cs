using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;

namespace bingGooAPI.Services
{
    public class CategoryService : IcategoryRepository
    {
        private readonly IDbConnection _connection;

        public CategoryService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var sql = @"SELECT Id, CategoryCode, CategoryName, Active,KhmerCategoryName, CreatedAt FROM Category";
            return await _connection.QueryAsync<Category>(sql);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            var sql = @"SELECT Id, CategoryCode, CategoryName, Active,KhmerCategoryName, CreatedAt FROM Category WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
        }

        public async Task<Category> CreateAsync(Category model)
        {
            // Check Category Name exists
            var checkSql = @"
        SELECT COUNT(1)
        FROM Category
        WHERE CategoryName = @CategoryName";

            var exists = await _connection.ExecuteScalarAsync<int>(
                checkSql,
                new { model.CategoryName });

            if (exists > 0)
            {
                throw new Exception($"Category Name :{model.CategoryName} Has already been used");
            }

            var sql = @"
        INSERT INTO Category
        (
            CategoryCode,
            CategoryName,
            KhmerCategoryName,
            Active,
            CreatedAt
        )
        VALUES
        (
            @CategoryCode,
            @CategoryName,
            @KhmerCategoryName,
            @Active,
            GETDATE()
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            model.CreatedAt = DateTime.Now;

            var id = await _connection.ExecuteScalarAsync<int>(sql, model);

            model.Id = id;

            return model;
        }

        private async Task ValidateCategoryNameAsync(string name)
        {

            var sql = @"
SELECT COUNT(1)
FROM Category
WHERE LOWER(CategoryName) = LOWER(@CategoryName)";
            var count = _connection.ExecuteScalar<int>(sql, new { CategoryName = name });
            if (count > 0)
            {
                throw new Exception("Category name already exists.");
            }
        }
        public async Task<bool> UpdateAsync(Category model)
        {
            var sql = @"
UPDATE Category SET
    CategoryCode = @CategoryCode,
    CategoryName = @CategoryName,
    Active = @Active,
    KhmerCategoryName = @KhmerCategoryName
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



