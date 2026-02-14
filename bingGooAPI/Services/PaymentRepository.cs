using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;

namespace bingGooAPI.Services
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
            var sql = @"
                INSERT INTO Payments
                (
                    OrderID,
                    PaymentMethod,
                    AmountPaid,
                    CashReceived,
                    PaymentStatus,
                    TransactionNo,
                    PaidAt,
                    CreatedAt
                )
                VALUES
                (
                    @OrderID,
                    @PaymentMethod,
                    @AmountPaid,
                    @CashReceived,
                    @PaymentStatus,
                    @TransactionNo,
                    GETDATE(),
                    GETDATE()
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _connection
                .QuerySingleAsync<int>(sql, payment);
        }


        public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
        {
            var sql = @"
                SELECT TOP 1 *
                FROM Payments
                WHERE OrderID = @OrderID
                ORDER BY CreatedAt DESC;
            ";

            return await _connection
                .QuerySingleOrDefaultAsync<Payment>(
                    sql,
                    new { OrderID = orderId }
                );
        }


        public async Task UpdatePaymentStatusAsync(
            int paymentId,
            string status,
            string? transactionNo = null)
        {
            var sql = @"
                UPDATE Payments
                SET
                    PaymentStatus = @Status,
                    TransactionNo = ISNULL(@TransactionNo, TransactionNo)
                WHERE PaymentID = @PaymentID;
            ";

            await _connection.ExecuteAsync(sql, new
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
            var sql = @"
                SELECT
                    PaymentID,
                    OrderID,
                    PaymentMethod,
                    AmountPaid,
                    CashReceived,
                    PaymentStatus,
                    TransactionNo,
                    PaidAt,
                    CreatedAt
                FROM Payments
                WHERE PaidAt BETWEEN @From AND @To
                ORDER BY PaidAt DESC;
            ";

            var list = await _connection
                .QueryAsync<Payment>(sql, new
                {
                    From = from,
                    To = to
                });

            return list.ToList();
        }
    }
}
