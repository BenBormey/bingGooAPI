using JuJuBiAPI.Entities;
using JuJuBiAPI.Models.Order;

namespace JuJuBiAPI.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);

        Task AddOrderItemAsync(OrderItem item);

        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<List<Order>> GetOrdersByUserAsync(int userId);

        Task<List<Order>> GetOrdersByOutletAsync(int outletId);

        Task UpdateOrderStatusAsync(int orderId, string status);

        Task<int> CheckoutAsync(int cartId);

        Task<PosCheckoutResult> PosCheckoutAsync(PosCheckoutRequest request);

        Task VoidOrderAsync(int orderId, string reason, string voidedBy);
    }
}
