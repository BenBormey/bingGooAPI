using Xunit;
using Moq;
using JuJuBiAPI.Controllers;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IO;
using System.Text;

namespace UnitTest
{
    public class ProductTest
    {
        private readonly Mock<IProductRepository> _repo;
        private readonly ProductController _controller;

        public ProductTest()
        {
            _repo = new Mock<IProductRepository>();
            _controller = new ProductController(_repo.Object);
        }

        private void SetOutletClaim(int outletId)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] { new Claim("OutletId", outletId.ToString()) }
                )
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                ProNumY = "P001",
                ProName = "Test Product"
            };

            var createdDto = new CreateProductDto
            {
                Id = 1,
                ProNumY = "P001",
                ProName = "Test Product"
            };

            _repo.Setup(r => r.CreateAsync(It.IsAny<CreateProductDto>()))
                 .ReturnsAsync(createdDto);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            var product = Assert.IsType<CreateProductDto>(createdResult.Value);

            Assert.Equal(1, product.Id);
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var list = new List<ProductListDto>
            {
                new ProductListDto
                {
                    ProID = 1,
                    ProName = "Test"
                }
            };

            _repo.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(list);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetForPOS_ReturnsOk()
        {
            SetOutletClaim(1);

            var list = new List<ProductPosDto>
            {
                new ProductPosDto
                {
                    ProductID = 1,
                    ProductName = "POS Product"
                }
            };

            _repo.Setup(r => r.GetForPosAsync(1, null))
                 .ReturnsAsync(list);

            var result = await _controller.GetForPOS(null);

            var ok = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task UploadImage_ReturnsOk()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            var file = new FormFile(stream, 0, stream.Length, "file", "test.txt");

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.UploadImage(file);

            var ok = Assert.IsType<OkObjectResult>(result);

            Assert.NotNull(ok.Value);
        }
    }
}