using bingGooAPI.Entities;

namespace bingGooAPI.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);

        Task AddOrderItemAsync(OrderItem item);

        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<List<Order>> GetOrdersByUserAsync(int userId);

        Task UpdateOrderStatusAsync(int orderId, string status);

        // ✅ For Checkout (Procedure)
        Task<int> CheckoutAsync(int cartId);
    }
}
