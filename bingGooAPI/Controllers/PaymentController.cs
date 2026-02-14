using System;
using System.Threading.Tasks;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Payment;
using Microsoft.AspNetCore.Mvc;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IOrderRepository _orderRepo;

        public PaymentController(
            IPaymentRepository paymentRepo,
            IOrderRepository orderRepo)
        {
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
        }

        [HttpPost("cash")]
        public async Task<IActionResult> CashPayment([FromBody] CashPaymentDto dto)
        {
            if (dto == null || dto.OrderId <= 0)
                return BadRequest("Invalid order id");

            await _orderRepo.UpdateOrderStatusAsync(dto.OrderId, "Paid");

            var payment = new Payment
            {
                OrderID = dto.OrderId,
                PaymentMethod = "Cash",
                AmountPaid = dto.Amount,
                CashReceived = dto.CashReceived,
                PaymentStatus = "Paid",
                TransactionNo = "CASH-" + DateTime.Now.Ticks
            };

            await _paymentRepo.CreatePaymentAsync(payment);

            return Ok(new
            {
                Message = "Cash payment success"
            });
        }

        [HttpPost("khqr")]
        public IActionResult GenerateKHQR([FromBody] KHQRRequestDto dto)
        {
            if (dto == null || dto.OrderId <= 0)
                return BadRequest("Invalid order id");

            string qrData =
                $"KHQR|ORDER:{dto.OrderId}|AMOUNT:{dto.Amount:0.00}";

            return Ok(new
            {
                QR = qrData,
                OrderId = dto.OrderId,
                Amount = dto.Amount
            });
        }

        [HttpPost("qr/confirm")]
        public async Task<IActionResult> ConfirmQrPayment([FromBody] QrConfirmDto dto)
        {
            if (dto == null || dto.OrderId <= 0)
                return BadRequest("Invalid order id");

            await _orderRepo.UpdateOrderStatusAsync(dto.OrderId, "Paid");

            var payment = new Payment
            {
                OrderID = dto.OrderId,
                PaymentMethod = "QR",
                AmountPaid = dto.Amount,
                CashReceived = Convert.ToInt32(null),
                PaymentStatus = "Paid",
                TransactionNo = dto.TransactionNo
            };

            await _paymentRepo.CreatePaymentAsync(payment);

            return Ok(new
            {
                Message = "QR payment confirmed"
            });
        }
    }
}
