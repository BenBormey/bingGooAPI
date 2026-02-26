using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
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
            var sql = @"
                SELECT * FROM Carts
                WHERE UserID = @UserID
                  AND Status = 'Active'";

            var cart = await _connection
                .QueryFirstOrDefaultAsync<Cart>(sql, new { UserID = userId });

            if (cart == null) return null;

            await LoadItems(cart);

            await RecalculateAndSave(cart);

            return cart;
        }


        public async Task<Cart> CreateCartAsync(Cart cart)
        {
            var sql = @"
                INSERT INTO Carts
                (
                    UserID, OutletID,
                    Status, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES
                (
                    @UserID, @OutletID,
                    @Status, GETDATE()
                )";

            var newCart = await _connection
                .QuerySingleAsync<Cart>(sql, cart);

            return newCart;
        }
        public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        {
            var sql = @"
                SELECT * FROM CartItems
                WHERE CartID = @CartID
                  AND ProductID = @ProductID";

            return await _connection
                .QueryFirstOrDefaultAsync<CartItem>(sql,
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

                var sql = @"
                INSERT INTO CartItems
                (
                    CartID, ProductID,
                    Quantity, UnitPrice,
                    DiscountPercent, DiscountAmount,
                    TaxPercent, TaxAmount,
                    SubTotal, TotalPrice,
                    CreatedAt
                )
                VALUES
                (
                    @CartID, @ProductID,
                    @Quantity, @UnitPrice,
                    @DiscountPercent, @DiscountAmount,
                    @TaxPercent, @TaxAmount,
                    @SubTotal, @TotalPrice,
                    GETDATE()
                )";

                await _connection.ExecuteAsync(sql, item);

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

            var sql = @"
                UPDATE CartItems
                SET
                    Quantity = @Quantity,
                    UnitPrice = @UnitPrice,
                    DiscountPercent = @DiscountPercent,
                    DiscountAmount = @DiscountAmount,
                    TaxPercent = @TaxPercent,
                    TaxAmount = @TaxAmount,
                    SubTotal = @SubTotal,
                    TotalPrice = @TotalPrice
                WHERE CartItemID = @CartItemID";

            await _connection.ExecuteAsync(sql, item);

            await UpdateCartTotal(item.CartID);
        }

        
        public async Task RemoveCartItemAsync(CartItem item)
        {
            var cartId = await _connection.ExecuteScalarAsync<int>(
                "SELECT CartID FROM CartItems WHERE CartItemID=@Id",
                new { Id = item.CartItemID });

            var sql = @"DELETE FROM CartItems WHERE CartItemID=@CartItemID";

            await _connection.ExecuteAsync(sql, item);

            await UpdateCartTotal(cartId);
        }

      
        public async Task UpdateCartAsync(Cart cart)
        {
            var sql = @"
                UPDATE Carts
                SET
                    SubTotal = @SubTotal,
                    DiscountAmount = @DiscountAmount,
                    TaxAmount = @TaxAmount,
                    GrandTotal = @GrandTotal,
                    UpdatedAt = GETDATE()
                WHERE CartID = @CartID";

            await _connection.ExecuteAsync(sql, cart);
        }

        public Task SaveAsync()
        {
            return Task.CompletedTask;
        }


        private async Task LoadItems(Cart cart)
        {
            var sql = @"SELECT * FROM CartItems WHERE CartID=@CartID";

            var items = await _connection
                .QueryAsync<CartItem>(sql, new { CartID = cart.CartID });

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
            var sql = @"
                SELECT
    ISNULL(SUM(SubTotal), 0) AS SubTotal,
    ISNULL(SUM(DiscountAmount), 0) AS DiscountAmount,
    ISNULL(SUM(TaxAmount), 0) AS TaxAmount,
    ISNULL(SUM(TotalPrice), 0) AS GrandTotal
    FROM CartItems
    WHERE CartID = @CartID;
";

            var totals = await _connection.QuerySingleAsync<Cart>(sql,
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
