using bingGooAPI.Models;
using bingGooAPI.Models.OutletProduct;

namespace bingGooAPI.Interfaces
{
    public interface IOutletProductRepository
    {
        // Menu grid: all products + this outlet's override
        Task<IEnumerable<OutletProductItem>> GetByOutletAsync(int outletId, string? search);

        // POS: only products this outlet can sell
        Task<IEnumerable<SellableProduct>> GetSellableAsync(int outletId);

        // Save one setting (insert or update)
        Task<int> UpsertAsync(OutletProductSave model);

        // Save many at once (e.g. the whole grid)
        Task<int> BulkUpsertAsync(IEnumerable<OutletProductSave> items);

        // Quick toggle
        Task<bool> SetCanSellAsync(int outletId, string proNumY, bool canSell);

        // Remove an override row
        Task<bool> DeleteAsync(int id);
    }
}
