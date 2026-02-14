using bingGooAPI.Entities;

namespace bingGooAPI.Interfaces
{
    public interface IPaymentRepository
    {

        Task<int> CreatePaymentAsync(Payment payment);

     
        Task<Payment?> GetPaymentByOrderIdAsync(int orderId);

        Task UpdatePaymentStatusAsync(
            int paymentId,
            string status,
            string? transactionNo = null
        );


        Task<List<Payment>> GetPaymentsByDateAsync(
            DateTime from,
            DateTime to
        );
    }
}
