using Xunit;
using Moq;
using JuJuBiAPI.Controllers;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Models.Outlet;
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

            var message = notFound.Value.GetType()
                .GetProperty("message")
                .GetValue(notFound.Value)
                .ToString();

            Assert.Equal("Outlet not found", message);
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
                     .ReturnsAsync(new Outlet());

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            var message = createdResult.Value.GetType()
                .GetProperty("message")
                .GetValue(createdResult.Value)
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

            var message = badRequest.Value.GetType()
                .GetProperty("message")
                .GetValue(badRequest.Value)
                .ToString();

            Assert.Equal("Id mismatch", message);
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
                     .ReturnsAsync(true);

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
                     .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var message = okResult.Value.GetType()
                .GetProperty("message")
                .GetValue(okResult.Value)
                .ToString();

            Assert.Equal("Outlet deleted successfully.", message);
        }
    }
}