using bingGooAPI.Controllers;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Castle.Components.DictionaryAdapter;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class CurrencyTest
    {
        private readonly Mock<IcurrencyRepository> _mockService;
        private readonly CurrencyController _controller;
        public CurrencyTest()
        {
            _mockService = new Mock<IcurrencyRepository>();
            _controller = new CurrencyController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithList()
        {
            var list = new List<Currency>
            {
                new Currency { CurrencyCode = "USD", CurrencyName = "US Dollar", BuyRate = 1.0m, SellRate = 1.0m, IsBase = true },
                new Currency { CurrencyCode = "EUR", CurrencyName = "Euro", BuyRate = 0.9m, SellRate = 0.9m, IsBase = false }
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

            var result = await _controller.GetAll();


            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<List<Currency>>(okResult.Value);

            Assert.Equal(2, data.Count);


        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var currency = new Currency
            {
                Id = 1,
                CurrencyCode = "USD",
                CurrencyName = "US Dollar",
                BuyRate = 1.0m,
                SellRate = 1.0m,
                IsBase = true
            };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(currency);

            var result = await _controller.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<Currency>(okResult.Value);
            Assert.Equal("USD", data.CurrencyCode);

        }
        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Currency)null);
            var result = await _controller.GetById(1);
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var currency = new Currency
            {
                Id = 1,
                CurrencyCode = "USD",
                CurrencyName = "US Dollar",
                BuyRate = 1.0m,
                SellRate = 1.0m,
                IsBase = true
            };
            _mockService.Setup(s => s.CreateAsync(currency)).ReturnsAsync(currency);
            var result = await _controller.Create(currency);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var data = Assert.IsType<Currency>(createdAtActionResult.Value);
            Assert.Equal("USD", data.CurrencyCode);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenSuccess()
        {
            var currency = new Currency
            {
                Id = 1,
                CurrencyCode = "USD",
                CurrencyName = "US Dollar",
                BuyRate = 1.0m,
                SellRate = 1.0m,
                IsBase = true
            };
            _mockService.Setup(s => s.UpdateAsync(currency)).ReturnsAsync(true);
            var result = await _controller.Update(1, currency);
            Assert.IsType<NoContentResult>(result);
        }
        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
                       var currency = new Currency
            {
                Id = 1,
                CurrencyCode = "USD",
                CurrencyName = "US Dollar",
                BuyRate = 1.0m,
                SellRate = 1.0m,
                IsBase = true
            };
            var result = await _controller.Update(2, currency);
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
           _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }
        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(d => d.DeleteAsync(1)).ReturnsAsync(false);
            var result = await _controller.Delete(1);
            Assert.IsType<NotFoundResult>(result);

        }
    }
}
