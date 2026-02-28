using Xunit;
using Moq;
using bingGooAPI.Controllers;
using bingGooAPI.Interfaces;
using bingGooAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTest
{
    public class CategoryTest
    {
        private readonly Mock<IcategoryRepository> _mockService;
        private readonly CategoryController _controller;

        public CategoryTest()
        {
            _mockService = new Mock<IcategoryRepository>();
            _controller = new CategoryController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithList()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, CategoryName = "Food" },
                new Category { Id = 2, CategoryName = "Drink" }
            };

            _mockService.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnData = Assert.IsType<List<Category>>(okResult.Value);

            Assert.Equal(2, returnData.Count);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var category = new Category
            {
                Id = 1,
                CategoryName = "Food"
            };

            _mockService.Setup(s => s.GetByIdAsync(1))
                        .ReturnsAsync(category);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnData = Assert.IsType<Category>(okResult.Value);

            Assert.Equal(1, returnData.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(1))
                        .ReturnsAsync((Category)null);

            var result = await _controller.GetById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedResult()
        {
            var category = new Category
            {
                Id = 1,
                CategoryName = "Food"
            };

            _mockService.Setup(s => s.CreateAsync(category))
                        .ReturnsAsync(category);

            var result = await _controller.Create(category);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnData = Assert.IsType<Category>(createdResult.Value);

            Assert.Equal("Food", returnData.CategoryName);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenSuccess()
        {
            var category = new Category
            {
                Id = 1,
                CategoryName = "Food Updated"
            };

            _mockService.Setup(s => s.UpdateAsync(category))
                        .ReturnsAsync(true);

            var result = await _controller.Update(1, category);

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