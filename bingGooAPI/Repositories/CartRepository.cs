using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IDbConnection _connection;

        public CartRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        public async Task<Cart?> GetActiveCartByUserAsync(int userId)
        {
            var cart = await _connection
                .QueryFirstOrDefaultAsync<Cart>(CartQueries.GetActiveCartByUser, new { UserID = userId });

            if (cart == null) return null;

            await LoadItems(cart);

            await RecalculateAndSave(cart);

            return cart;
        }


        public async Task<Cart> CreateCartAsync(Cart cart)
        {
            var newCart = await _connection
                .QuerySingleAsync<Cart>(CartQueries.CreateCart, cart);

            return newCart;
        }
        public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        {
            return await _connection
                .QueryFirstOrDefaultAsync<CartItem>(CartQueries.GetCartItem,
                new
                {
                    CartID = cartId,
                    ProductID = productId
                });
        }


        public async Task AddCartItemAsync(CartItem item)
        {
            try
            {
                CalculateItem(item);

                await _connection.ExecuteAsync(CartQueries.AddCartItem, item);

               await UpdateCartTotal(item.CartID);
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error adding cart item: {ex.Message}");
                throw;
            }

        }


        public async Task UpdateCartItemAsync(CartItem item)
        {
            CalculateItem(item);

            await _connection.ExecuteAsync(CartQueries.UpdateCartItem, item);

            await UpdateCartTotal(item.CartID);
        }


        public async Task RemoveCartItemAsync(CartItem item)
        {
            var cartId = await _connection.ExecuteScalarAsync<int>(
                CartQueries.GetCartIdByItem,
                new { Id = item.CartItemID });

            await _connection.ExecuteAsync(CartQueries.RemoveCartItem, item);

            await UpdateCartTotal(cartId);
        }


        public async Task UpdateCartAsync(Cart cart)
        {
            await _connection.ExecuteAsync(CartQueries.UpdateCart, cart);
        }

        private async Task LoadItems(Cart cart)
        {
            var items = await _connection
                .QueryAsync<CartItem>(CartQueries.LoadItems, new { CartID = cart.CartID });

            cart.CartItems = items.ToList();
        }

        private void CalculateItem(CartItem item)
        {
            item.SubTotal = item.Quantity * item.UnitPrice;

            item.DiscountAmount =
                item.SubTotal * item.DiscountPercent / 100;

            var afterDiscount =
                item.SubTotal - item.DiscountAmount;

            item.TaxAmount =
                afterDiscount * item.TaxPercent / 100;

            item.TotalPrice =
                afterDiscount + item.TaxAmount;
        }

        private async Task UpdateCartTotal(int cartId)
        {
            var totals = await _connection. QuerySingleAsync<Cart>(CartQueries.CartTotals,
                new { CartID = cartId });

            totals.CartID = cartId;

            await UpdateCartAsync(totals);
        }
        private async Task RecalculateAndSave(Cart cart)
        {
            cart.SubTotal = cart.CartItems.Sum(x => x.SubTotal);
            cart.DiscountAmount = cart.CartItems.Sum(x => x.DiscountAmount);
            cart.TaxAmount = cart.CartItems.Sum(x => x.TaxAmount);
            cart.GrandTotal = cart.CartItems.Sum(x => x.TotalPrice);

            await UpdateCartAsync(cart);
        }
    }
}
