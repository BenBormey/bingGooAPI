using Xunit;
using Moq;
using bingGooAPI.Controllers;
using bingGooAPI.Interfaces;
using bingGooAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace UnitTest
{
    public class OrderTest
    {
        private readonly Mock<IOrderRepository> _mockRepo;
        private readonly OrderController _controller;

        public OrderTest()
        {
            _mockRepo = new Mock<IOrderRepository>();
            _controller = new OrderController(_mockRepo.Object);
        }

        [Fact]
        public async Task Checkout_ReturnsBadRequest_WhenCartIdInvalid()
        {
            // Act
            var result = await _controller.Checkout(0);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("Invalid cart id", badRequest.Value);
        }

        [Fact]
        public async Task Checkout_ReturnsOk_WhenSuccess()
        {
            // Arrange
            _mockRepo.Setup(x => x.CheckoutAsync(1))
                     .ReturnsAsync(100);

            // Act
            var result = await _controller.Checkout(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);

            var message = ok.Value.GetType()
                .GetProperty("Message")
                .GetValue(ok.Value)
                .ToString();

            var orderId = ok.Value.GetType()
                .GetProperty("OrderID")
                .GetValue(ok.Value);

            Assert.Equal("Checkout success", message);
            Assert.Equal(100, orderId);
        }

        [Fact]
        public async Task Checkout_Returns500_WhenExceptionThrown()
        {
            // Arrange
            _mockRepo.Setup(x => x.CheckoutAsync(1))
                     .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Checkout(1);

            // Assert
            var error = Assert.IsType<ObjectResult>(result);

            Assert.Equal(500, error.StatusCode);
        }

        [Fact]
        public async Task GetOrder_ReturnsOk_WhenFound()
        {
            // Arrange
            var order = new Order
            {
                OrderID = 1
            };

            _mockRepo.Setup(x => x.GetOrderByIdAsync(1))
                     .ReturnsAsync(order);

            // Act
            var result = await _controller.GetOrder(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);

            var data = Assert.IsType<Order>(ok.Value);

            Assert.Equal(1, data.OrderID);
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetOrderByIdAsync(1))
                     .ReturnsAsync((Order)null);

            // Act
            var result = await _controller.GetOrder(1);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal("Order not found", notFound.Value);
        }

        [Fact]
        public async Task GetUserOrders_ReturnsOk()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { OrderID = 1 },
                new Order { OrderID = 2 }
            };

            _mockRepo.Setup(x => x.GetOrdersByUserAsync(1))
                     .ReturnsAsync(orders);

            // Act
            var result = await _controller.GetUserOrders(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);

            var data = Assert.IsType<List<Order>>(ok.Value);

            Assert.Equal(2, data.Count);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsBadRequest_WhenStatusEmpty()
        {
            // Act
            var result = await _controller.UpdateStatus(1, "");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("Status is required", badRequest.Value);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsOk_WhenSuccess()
        {
            // Arrange
            _mockRepo.Setup(x => x.UpdateOrderStatusAsync(1, "Completed"))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateStatus(1, "Completed");

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);

            Assert.Equal("Status updated", ok.Value);
        }
    }
}