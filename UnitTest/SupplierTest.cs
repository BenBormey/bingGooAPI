using Xunit;
using Moq;
using bingGooAPI.Controllers;
using bingGooAPI.Interfaces;
using bingGooAPI.Entities;
using bingGooAPI.Models.Supplier;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTest
{
    public class SupplierTest
    {
        private readonly Mock<ISupplierRepository> _mockRepo;
        private readonly SupplierController _controller;

        public SupplierTest()
        {
            _mockRepo = new Mock<ISupplierRepository>();
            _controller = new SupplierController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithList()
        {
            // Arrange
            var list = new List<Supplier>
            {
                new Supplier { SupplierID = 1, SupplierName = "Supplier A" },
                new Supplier { SupplierID = 2, SupplierName = "Supplier B" }
            };

            _mockRepo.Setup(x => x.GetAllAsync())
                     .ReturnsAsync(list);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);

            var data = Assert.IsType<List<Supplier>>(ok.Value);

            Assert.Equal(2, data.Count);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var supplier = new Supplier
            {
                SupplierID = 1,
                SupplierName = "Supplier A"
            };

            _mockRepo.Setup(x => x.GetByIdAsync(1))
                     .ReturnsAsync(supplier);

            var result = await _controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result);

            var data = Assert.IsType<Supplier>(ok.Value);

            Assert.Equal("Supplier A", data.SupplierName);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            _mockRepo.Setup(x => x.GetByIdAsync(1))
                     .ReturnsAsync((Supplier)null);

            var result = await _controller.GetById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal("Supplier not found", notFound.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var dto = new CreateSupplierDto
            {
                SupplierCode = "SUP001",
                SupplierName = "Supplier A",
                ContactName = "John",
                Phone = "123",
                Email = "test@test.com",
                Address = "Phnom Penh",
                TaxNumber = "TX001"
            };

            var created = new Supplier
            {
                SupplierID = 1,
                SupplierName = "Supplier A"
            };

            _mockRepo.Setup(x => x.CreateAsync(It.IsAny<Supplier>()))
                     .ReturnsAsync(created);

            var result = await _controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            var supplier = Assert.IsType<Supplier>(createdResult.Value);

            Assert.Equal(1, supplier.SupplierID);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenSupplierMissing()
        {
            var dto = new UpdateSupplierDto
            {
                SupplierCode = "SUP001",
                SupplierName = "Supplier A"
            };

            _mockRepo.Setup(x => x.GetByIdAsync(1))
                     .ReturnsAsync((Supplier)null);

            var result = await _controller.Update(1, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal("Supplier not found", notFound.Value);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccess()
        {
            var supplier = new Supplier
            {
                SupplierID = 1,
                SupplierName = "Supplier A"
            };

            var dto = new UpdateSupplierDto
            {
                SupplierCode = "SUP001",
                SupplierName = "Updated Supplier",
                Status = true
            };

            _mockRepo.Setup(x => x.GetByIdAsync(1))
                     .ReturnsAsync(supplier);

            _mockRepo.Setup(x => x.UpdateAsync(It.IsAny<Supplier>()))
                     .ReturnsAsync(true);

            var result = await _controller.Update(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);

            var data = Assert.IsType<Supplier>(ok.Value);

            Assert.Equal("Updated Supplier", data.SupplierName);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            _mockRepo.Setup(x => x.GetByIdAsync(1))
                     .ReturnsAsync((Supplier)null);

            var result = await _controller.Delete(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal("Supplier not found", notFound.Value);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccess()
        {
            var supplier = new Supplier
            {
                SupplierID = 1,
                SupplierName = "Supplier A"
            };

            _mockRepo.Setup(x => x.GetByIdAsync(1))
                     .ReturnsAsync(supplier);

            _mockRepo.Setup(x => x.DeleteAsync(1))
                     .ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var ok = Assert.IsType<OkObjectResult>(result);

            Assert.Equal("Deleted successfully", ok.Value);
        }
    }
}