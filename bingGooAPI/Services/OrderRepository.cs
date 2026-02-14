using System.Data;
using Dapper;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;

namespace bingGooAPI.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnection _connection;

        public OrderRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        // ✅ Create Order (Manual Insert)
        public async Task<Order> CreateOrderAsync(Order order)
        {
            var sql = @"
                INSERT INTO Orders
                (
                    CartID,
                    UserID,
                    OutletID,
                    SubTotal,
                    DiscountAmount,
                    TaxAmount,
                    GrandTotal,
                    OrderStatus,
                    CreatedAt
                )
                VALUES
                (
                    @CartID,
                    @UserID,
                    @OutletID,
                    @SubTotal,
                    @DiscountAmount,
                    @TaxAmount,
                    @GrandTotal,
                    @OrderStatus,
                    GETDATE()
                );

                SELECT *
                FROM Orders
                WHERE OrderID = CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _connection
                .QuerySingleAsync<Order>(sql, order);
        }

        // ✅ Insert OrderItem
        public async Task AddOrderItemAsync(OrderItem item)
        {
            var sql = @"
                INSERT INTO OrderItems
                (
                    OrderID,
                    ProductID,
                    Quantity,
                    UnitPrice,
                    DiscountPercent,
                    DiscountAmount,
                    TaxPercent,
                    TaxAmount,
                    SubTotal,
                    TotalPrice,
                    CreatedAt
                )
                VALUES
                (
                    @OrderID,
                    @ProductID,
                    @Quantity,
                    @UnitPrice,
                    @DiscountPercent,
                    @DiscountAmount,
                    @TaxPercent,
                    @TaxAmount,
                    @SubTotal,
                    @TotalPrice,
                    GETDATE()
                );
            ";

            await _connection.ExecuteAsync(sql, item);
        }

        // ✅ Get Order + Items
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            var sql = @"
                SELECT *
                FROM Orders
                WHERE OrderID = @OrderID;

                SELECT *
                FROM OrderItems
                WHERE OrderID = @OrderID;
            ";

            using var multi =
                await _connection.QueryMultipleAsync(
                    sql,
                    new { OrderID = orderId });

            var order =
                await multi.ReadSingleOrDefaultAsync<Order>();

            if (order == null)
                return null;

            order.OrderItems =
                (await multi.ReadAsync<OrderItem>()).ToList();

            return order;
        }

        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            var sql = @"
                SELECT *
                FROM Orders
                WHERE UserID = @UserID
                ORDER BY CreatedAt DESC;
            ";

            var orders = await _connection
                .QueryAsync<Order>(sql, new { UserID = userId });

            return orders.ToList();
        }

        // ✅ Update Status
        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var sql = @"
                UPDATE Orders
                SET
                    OrderStatus = @Status,
                    UpdatedAt = GETDATE()
                WHERE OrderID = @OrderID;
            ";

            await _connection.ExecuteAsync(sql, new
            {
                OrderID = orderId,
                Status = status
            });
        }

        // ✅ Checkout Using Stored Procedure
        public async Task<int> CheckoutAsync(int cartId)
        {
            var orderId = await _connection
                .QuerySingleAsync<int>(
                    "CheckoutCart",
                    new { CartID = cartId },
                    commandType: CommandType.StoredProcedure
                );

            return orderId;
        }
    }
}
