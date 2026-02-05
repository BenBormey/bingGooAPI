using bingGooAPI.Entities;

namespace bingGooAPI.Interfaces
{
    public interface ICartRepository
    {
     
        Task<Cart?> GetActiveCartByUserAsync(int userId);

 
        Task<Cart> CreateCartAsync(Cart cart);


        Task<CartItem?> GetCartItemAsync(int cartId, int productId);


        Task AddCartItemAsync(CartItem item);

   
        Task UpdateCartItemAsync(CartItem item);

        Task RemoveCartItemAsync(CartItem item);

   
        Task UpdateCartAsync(Cart cart);

        Task SaveAsync();
    }
}
