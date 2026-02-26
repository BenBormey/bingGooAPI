using System.Threading.Tasks;
using bingGooAPI.Controllers;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTest
{
    public class PaymentTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepoMock;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly PaymentController _controller;

        public PaymentTests()
        {
            _paymentRepoMock = new Mock<IPaymentRepository>();
            _orderRepoMock = new Mock<IOrderRepository>();

            _controller = new PaymentController(
                _paymentRepoMock.Object,
                _orderRepoMock.Object);
        }

        // Cash Payment Success
        [Fact]
        public async Task CashPayment_ValidOrder_ReturnsOk()
        {
            var dto = new CashPaymentDto
            {
                OrderId = 1,
                Amount = 10,
                CashReceived = 10
            };

            _orderRepoMock
                .Setup(x => x.UpdateOrderStatusAsync(dto.OrderId, "Paid"))
                .Returns(Task.CompletedTask); // because UpdateOrderStatusAsync returns Task

            _paymentRepoMock
                .Setup(x => x.CreatePaymentAsync(It.IsAny<Payment>()))
                .ReturnsAsync(1); // because CreatePaymentAsync returns Task<int>

            var result = await _controller.CashPayment(dto);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CashPayment_InvalidOrder_ReturnsBadRequest()
        {
            var dto = new CashPaymentDto
            {
                OrderId = 0
            };

            var result = await _controller.CashPayment(dto);

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void GenerateKHQR_ReturnsOk()
        {
            var dto = new KHQRRequestDto
            {
                OrderId = 1,
                Amount = 20
            };

            var result = _controller.GenerateKHQR(dto);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task ConfirmQrPayment_ReturnsOk()
        {
            var dto = new QrConfirmDto
            {
                OrderId = 1,
                Amount = 20,
                TransactionNo = "TXN123"
            };

            _orderRepoMock
             .Setup(x => x.UpdateOrderStatusAsync(dto.OrderId, "Paid"))
             .Returns(Task.CompletedTask);  

            _paymentRepoMock
                .Setup(x => x.CreatePaymentAsync(It.IsAny<Payment>()))
                .ReturnsAsync(1);            

            var result = await _controller.ConfirmQrPayment(dto);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            _orderRepoMock.Verify(
                x => x.UpdateOrderStatusAsync(dto.OrderId, "Paid"),
                Times.Once);

            _paymentRepoMock.Verify(
                x => x.CreatePaymentAsync(It.IsAny<Payment>()),
                Times.Once);
        }
    }
}