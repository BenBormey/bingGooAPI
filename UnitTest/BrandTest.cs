using Xunit;
using Moq;
using bingGooAPI.Controllers;
using bingGooAPI.Interfaces;
using bingGooAPI.Entities;
using bingGooAPI.Models.Branch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTest
{
    public class BrandTest
    {
        private readonly Mock<IbrandRepository> _mockService;
        private readonly BrandController _controller;

        public BrandTest()
        {
            _mockService = new Mock<IbrandRepository>();
            _controller = new BrandController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithList()
        {
            // Arrange
            var list = new List<Branch>
            {
                new Branch { Id = 1, BranchName = "Brand A" },
                new Branch { Id = 2, BranchName = "Brand B" }
            };

            _mockService.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(list);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<List<Branch>>(okResult.Value);

            Assert.Equal(2, data.Count);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var branch = new Branch
            {
                Id = 1,
                BranchName = "Brand A"
            };

            _mockService.Setup(s => s.GetByIdAsync(1))
                        .ReturnsAsync(branch);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<Branch>(okResult.Value);

            Assert.Equal(1, data.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.GetByIdAsync(1))
                        .ReturnsAsync((Branch)null);

            var result = await _controller.GetById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var model = new CreateBranch
            {
                BranchName = "Brand New"
            };

            var created = new Branch
            {
                Id = 1,
                BranchName = "Brand New"
            };

            _mockService.Setup(s => s.CreateAsync(model))
                        .ReturnsAsync(created);

            var result = await _controller.Create(model);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var data = Assert.IsType<Branch>(createdResult.Value);

            Assert.Equal("Brand New", data.BranchName);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenSuccess()
        {
            var branch = new Branch
            {
                Id = 1,
                BranchName = "Updated Brand"
            };

            _mockService.Setup(s => s.UpdateAsync(branch))
                        .ReturnsAsync(true);

            var result = await _controller.Update(1, branch);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            _mockService.Setup(s => s.DeleteAsync(1))
                        .ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}