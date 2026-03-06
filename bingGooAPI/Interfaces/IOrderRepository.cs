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

        Task<int> CheckoutAsync(int cartId);
    }
}
