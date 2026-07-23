using System;
using System.Threading.Tasks;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
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

            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than zero");

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

            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than zero");

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

            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than zero");

            await _orderRepo.UpdateOrderStatusAsync(dto.OrderId, "Paid");

            var payment = new Payment
            {
                OrderID = dto.OrderId,
                PaymentMethod = "QR",
                AmountPaid = dto.Amount,
                CashReceived = 0,
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
