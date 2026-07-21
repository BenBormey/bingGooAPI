using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;

namespace JuJuBiAPI.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbConnection _connection;

        public PaymentRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        public async Task<int> CreatePaymentAsync(Payment payment)
        {
            return await _connection
                .QuerySingleAsync<int>(PaymentQueries.Create, payment);
        }


        public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _connection
                .QuerySingleOrDefaultAsync<Payment>(
                    PaymentQueries.GetByOrderId,
                    new { OrderID = orderId }
                );
        }


        public async Task UpdatePaymentStatusAsync(
            int paymentId,
            string status,
            string? transactionNo = null)
        {
            await _connection.ExecuteAsync(PaymentQueries.UpdateStatus, new
            {
                PaymentID = paymentId,
                Status = status,
                TransactionNo = transactionNo
            });
        }


        public async Task<List<Payment>> GetPaymentsByDateAsync(
            DateTime from,
            DateTime to)
        {
            var list = await _connection
                .QueryAsync<Payment>(PaymentQueries.GetByDate, new
                {
                    From = from,
                    To = to
                });

            return list.ToList();
        }
    }
}
