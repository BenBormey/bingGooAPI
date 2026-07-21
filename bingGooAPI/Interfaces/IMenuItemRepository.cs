using JuJuBiAPI.Models;
using JuJuBis.Domain.Entities;

namespace JuJuBiAPI.Interfaces
{
    public interface IMenuItemRepository
    {

        Task<IEnumerable<MenuItem>> GetAllAsync();


        Task<MenuItem?> GetByIdAsync(int menuItemId);

  
        Task<IEnumerable<MenuItem>> GetByOutletAsync(int outletId);

       
        Task<MenuItem?> GetByOutletAndProductAsync(int outletId, string proNumY);

 
        Task<bool> ExistsAsync(int outletId, string proNumY);

   
        Task<MenuItem> CreateAsync(MenuItem menuItem);

  
        Task<bool> UpdateAsync(MenuItem menuItem);

   
        Task<bool> DeleteAsync(int menuItemId);

      
        Task<bool> SetActiveAsync(int menuItemId, bool isActive, string updatedBy);

        Task<int> SetOutletDiscountAsync(
            int outletId, decimal percent, DateTime? startDate, DateTime? endDate, string updatedBy);

        Task<int> ClearOutletDiscountAsync(int outletId, string updatedBy);
    }
}