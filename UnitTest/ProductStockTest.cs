using Xunit;
using Moq;
using bingGooAPI.Controllers;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.ProductStock;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTest
{
    public class ProductStockTest
    {
        private readonly Mock<IProductStockRepository> _mockService;
        private readonly ProductStockController _controller;

        public ProductStockTest()
        {
            _mockService = new Mock<IProductStockRepository>();
            _controller = new ProductStockController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenSuccess()
        {
            var dto = new CreateProductStockDto
            {
                ProductID = 1,
                BranchId = 1,
                OutletId = 1,
                StockQty = 10
            };

            _mockService.Setup(x => x.CreateAsync(dto))
                        .ReturnsAsync(100);

            var result = await _controller.Create(dto);

            var ok = Assert.IsType<OkObjectResult>(result);

            var message = ok.Value.GetType()
                .GetProperty("Message")
                .GetValue(ok.Value)
                .ToString();

            var stockId = ok.Value.GetType()
                .GetProperty("StockID")
                .GetValue(ok.Value);

            Assert.Equal("Product stock created successfully", message);
            Assert.Equal(100, stockId);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithList()
        {
            var list = new List<ProductStockDto>
            {
                new ProductStockDto { StockID = 1, StockQty = 10 },
                new ProductStockDto { StockID = 2, StockQty = 20 }
            };

            _mockService.Setup(x => x.GetAllAsync())
                        .ReturnsAsync(list);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);

            var data = Assert.IsType<List<ProductStockDto>>(ok.Value);

            Assert.Equal(2, data.Count);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var dto = new ProductStockDto
            {
                StockID = 1,
                StockQty = 10
            };

            _mockService.Setup(x => x.GetByIdAsync(1))
                        .ReturnsAsync(dto);

            var result = await _controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result);

            var data = Assert.IsType<ProductStockDto>(ok.Value);

            Assert.Equal(1, data.StockID);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(x => x.GetByIdAsync(1))
                        .ReturnsAsync((ProductStockDto)null);

            var result = await _controller.GetById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            var message = notFound.Value.GetType()
                .GetProperty("Message")
                .GetValue(notFound.Value)
                .ToString();

            Assert.Equal("Stock not found", message);
        }

        [Fact]
        public async Task Search_ReturnsOk_WhenFound()
        {
            var dto = new ProductStockDto
            {
                StockID = 1,
                StockQty = 10
            };

            _mockService.Setup(x => x.GetByProductBranchOutletAsync(1, 1, 1))
                        .ReturnsAsync(dto);

            var result = await _controller.GetByProductBranchOutlet(1, 1, 1);

            var ok = Assert.IsType<OkObjectResult>(result);

            var data = Assert.IsType<ProductStockDto>(ok.Value);

            Assert.Equal(1, data.StockID);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var dto = new UpdateProductStockDto
            {
                StockID = 2,
                StockQty  =20           };

            var result = await _controller.Update(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("ID mismatch", badRequest.Value);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccess()
        {
            var dto = new UpdateProductStockDto
            {
                StockID = 1,
                StockQty = 50
            };

            _mockService.Setup(x => x.UpdateAsync(dto))
                        .ReturnsAsync(true);

            var result = await _controller.Update(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);

            var message = ok.Value.GetType()
                .GetProperty("Message")
                .GetValue(ok.Value)
                .ToString();

            Assert.Equal("Product stock updated successfully", message);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccess()
        {
            _mockService.Setup(x => x.DeleteAsync(1))
                        .ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var ok = Assert.IsType<OkObjectResult>(result);

            var message = ok.Value.GetType()
                .GetProperty("Message")
                .GetValue(ok.Value)
                .ToString();

            Assert.Equal("Product stock deleted successfully", message);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenFailed()
        {
            _mockService.Setup(x => x.DeleteAsync(1))
                        .ReturnsAsync(false);

            var result = await _controller.Delete(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            var message = notFound.Value.GetType()
                .GetProperty("Message")
                .GetValue(notFound.Value)
                .ToString();

            Assert.Equal("Delete failed", message);
        }
    }
}