using Xunit;
using Moq;
using bingGooAPI.Controllers;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Outlet;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTest
{
    public class OutletTest
    {
        private readonly Mock<IOutletRepository> _mockRepo;
        private readonly OutletController _controller;

        public OutletTest()
        {
            _mockRepo = new Mock<IOutletRepository>();
            _controller = new OutletController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithOutletList()
        {
            // Arrange
            var list = new List<OutletListDto>
            {
                new OutletListDto { Id = 1, OutletName = "Outlet A" },
                new OutletListDto { Id = 2, OutletName = "Outlet B" }
            };

            _mockRepo.Setup(x => x.GetAllAsync())
                     .ReturnsAsync(list);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<List<OutletListDto>>(okResult.Value);

            Assert.Equal(2, data.Count);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            // Arrange
            var outlet = new OutletListDto
            {
                Id = 1,
                OutletName = "Outlet A"
            };

            _mockRepo.Setup(x => x.GetByIdAsync(1))
                     .ReturnsAsync(outlet);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<OutletListDto>(okResult.Value);

            Assert.Equal("Outlet A", data.OutletName);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetByIdAsync(1))
                     .ReturnsAsync((OutletListDto)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);

            Assert.Equal("Outlet not found", notFound.Value);
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var request = new CreateOutletDtos
            {
                OutletName = "New Outlet"
            };

            _mockRepo.Setup(x => x.AddAsync(request))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var message = okResult.Value.GetType()
                .GetProperty("message")
                .GetValue(okResult.Value)
                .ToString();

            Assert.Equal("Create Outlet complete", message);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var request = new UpdateOutletDto
            {
                Id = 2,
                OutletName = "Updated Outlet"
            };

            // Act
            var result = await _controller.Update(1, request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("Id mismatch", badRequest.Value);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var request = new UpdateOutletDto
            {
                Id = 1,
                OutletName = "Updated Outlet"
            };

            _mockRepo.Setup(x => x.UpdateAsync(request))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(1, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var message = okResult.Value.GetType()
                .GetProperty("message")
                .GetValue(okResult.Value)
                .ToString();

            Assert.Equal("Update Outlet complete", message);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccess()
        {
            // Arrange
            _mockRepo.Setup(x => x.DeleteAsync(1))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var message = okResult.Value.GetType()
                .GetProperty("message")
                .GetValue(okResult.Value)
                .ToString();

            Assert.Equal("Delete Outlet complete", message);
        }
    }
}