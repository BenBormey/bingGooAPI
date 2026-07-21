using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;
using Dapper;
using JuJuBis.Domain.Entities;
using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly IDbConnection _connection;

        public MenuItemRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _connection.QueryAsync<MenuItem>(MenuItemQueries.GetAll);
        }

        public async Task<MenuItem?> GetByIdAsync(int menuItemId)
        {
            return await _connection.QueryFirstOrDefaultAsync<MenuItem>(
                MenuItemQueries.GetById,
                new { MenuItemId = menuItemId });
        }

        public async Task<IEnumerable<MenuItem>> GetByOutletAsync(int outletId)
        {
            return await _connection.QueryAsync<MenuItem>(
                MenuItemQueries.GetByOutlet,
                new { OutletId = outletId });
        }

        public async Task<MenuItem?> GetByOutletAndProductAsync(int outletId, string proNumY)
        {
            return await _connection.QueryFirstOrDefaultAsync<MenuItem>(
                MenuItemQueries.GetByOutletAndProduct,
                new
                {
                    OutletId = outletId,
                    ProNumY = proNumY
                });
        }

        public async Task<bool> ExistsAsync(int outletId, string proNumY)
        {
            var count = await _connection.ExecuteScalarAsync<int>(
                MenuItemQueries.Exists,
                new
                {
                    OutletId = outletId,
                    ProNumY = proNumY
                });

            return count > 0;
        }

        public async Task<MenuItem> CreateAsync(MenuItem model)
        {
            return await _connection.QuerySingleAsync<MenuItem>(MenuItemQueries.Create, model);
        }

        public async Task<bool> UpdateAsync(MenuItem model)
        {
            return await _connection.ExecuteAsync(MenuItemQueries.Update, model) > 0;
        }

        public async Task<bool> DeleteAsync(int menuItemId)
        {
            var rows = await _connection.ExecuteAsync(
                MenuItemQueries.Delete,
                new { MenuItemId = menuItemId });

            return rows > 0;
        }

        public async Task<int> SetOutletDiscountAsync(
            int outletId, decimal percent, DateTime? startDate, DateTime? endDate, string updatedBy)
        {
            return await _connection.ExecuteAsync(MenuItemQueries.SetOutletDiscount, new
            {
                OutletId = outletId,
                Percent = percent,
                StartDate = startDate,
                EndDate = endDate,
                UpdatedBy = updatedBy
            });
        }

        public async Task<int> ClearOutletDiscountAsync(int outletId, string updatedBy)
        {
            return await _connection.ExecuteAsync(MenuItemQueries.ClearOutletDiscount, new
            {
                OutletId = outletId,
                UpdatedBy = updatedBy
            });
        }

        public async Task<bool> SetActiveAsync(int menuItemId, bool isActive, string updatedBy)
        {
            var rows = await _connection.ExecuteAsync(
                MenuItemQueries.SetActive,
                new
                {
                    MenuItemId = menuItemId,
                    IsActive = isActive,
                    UpdatedBy = updatedBy
                });

            return rows > 0;
        }
    }
}
