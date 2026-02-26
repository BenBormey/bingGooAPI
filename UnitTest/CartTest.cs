using bingGooAPI.Controllers;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Cart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest
{
    public class CartTest
    {
        private readonly Mock<ICartRepository> _mockRepo;
        private readonly CartController _controller;

        public CartTest()
        {
            _mockRepo = new Mock<ICartRepository>();
            _controller = new CartController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetCart_ReturnsOk_WithCart()
        {
          
            var cart = new Cart
            {
                CartID = 1,
                UserID = 1,
                Status = "Active",
                CartItems = new List<CartItem>()
            };

            _mockRepo.Setup(x => x.GetActiveCartByUserAsync(1))
                     .ReturnsAsync(cart);

            var result = await _controller.GetCart(1);


            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnCart = Assert.IsType<Cart>(okResult.Value);

            Assert.Equal(1, returnCart.UserID);
        }

        [Fact]
        public async Task AddToCart_CreatesNewCart_WhenCartNotExists()
        {
           
            var request = new AddToCartRequest
            {
                UserId = 1,
                OutletId = 1,
                ProductId = 10,
                Quantity = 2,
                UnitPrice = 5,
                DiscountPercent = 0,
                TaxPercent = 0
            };

            var newCart = new Cart
            {
                CartID = 1,
                UserID = 1,
                CartItems = new List<CartItem>()
            };

            _mockRepo.Setup(x => x.GetActiveCartByUserAsync(request.UserId))
                     .ReturnsAsync((Cart)null);

            _mockRepo.Setup(x => x.CreateCartAsync(It.IsAny<Cart>()))
                     .ReturnsAsync(newCart);

            _mockRepo.Setup(x => x.GetCartItemAsync(newCart.CartID, request.ProductId))
                     .ReturnsAsync((CartItem)null);

            _mockRepo.Setup(x => x.AddCartItemAsync(It.IsAny<CartItem>()))
                     .Returns(Task.CompletedTask);

            _mockRepo.Setup(x => x.UpdateCartAsync(It.IsAny<Cart>()))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddToCart(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item added to cart", okResult.Value);
        }

        [Fact]
        public async Task UpdateItem_ReturnsNotFound_WhenItemMissing()
        {
            // Arrange
            var request = new UpdateCartItemRequest
            {
                CartId = 1,
                ProductId = 10,
                Quantity = 5
            };

            _mockRepo.Setup(x => x.GetCartItemAsync(request.CartId, request.ProductId))
                     .ReturnsAsync((CartItem)null);

       
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

         
            var result = await _controller.UpdateItem(request);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Item not found", notFound.Value);
        }

        [Fact]
        public async Task RemoveItem_ReturnsOk()
        {
            _mockRepo.Setup(x => x.RemoveCartItemAsync(It.IsAny<CartItem>()))
                     .Returns(Task.CompletedTask);

            var result = await _controller.RemoveItem(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item removed", okResult.Value);
        }

        [Fact]
        public async Task AddToCart_UpdatesQuantity_WhenItemExists()
        {
            var cart = new Cart
            {
                CartID = 1,
                UserID = 1,
                CartItems = new List<CartItem>()
            };

            var existingItem = new CartItem
            {
                CartID = 1,
                ProductID = 10,
                Quantity = 1,
                UnitPrice = 5,
                DiscountPercent = 0,
                TaxPercent = 0
            };

            var request = new AddToCartRequest
            {
                UserId = 1,
                OutletId = 1,
                ProductId = 10,
                Quantity = 2,
                UnitPrice = 5,
                DiscountPercent = 0,
                TaxPercent = 0
            };

            _mockRepo.Setup(x => x.GetActiveCartByUserAsync(request.UserId))
                     .ReturnsAsync(cart);

            _mockRepo.Setup(x => x.GetCartItemAsync(cart.CartID, request.ProductId))
                     .ReturnsAsync(existingItem);

            _mockRepo.Setup(x => x.UpdateCartItemAsync(It.IsAny<CartItem>()))
                     .Returns(Task.CompletedTask);

            _mockRepo.Setup(x => x.UpdateCartAsync(It.IsAny<Cart>()))
                     .Returns(Task.CompletedTask);

            var result = await _controller.AddToCart(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item added to cart", okResult.Value);
        }
    }
}