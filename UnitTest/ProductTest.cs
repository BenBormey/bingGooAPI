using Xunit;
using Moq;
using bingGooAPI.Controllers;
using bingGooAPI.Interfaces;
using bingGooAPI.Entities;
using bingGooAPI.Models.Product;
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
                ProductCode = "P001",
                ProductName = "Test Product",
                BrandId = 1,
                CategoryId = 1,
                SupplierId = 1,
                SellingPrice = 10,
                Status = true
            };

            var createdEntity = new Product
            {
                ProductID = 1,
                ProductName = "Test Product"
            };

            // IMPORTANT FIX: specify Product explicitly
            _repo.Setup(r => r.CreateAsync(It.IsAny<Product>()))
                 .Returns(Task.FromResult<Product>(createdEntity));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            var product = Assert.IsType<Product>(createdResult.Value);

            Assert.Equal(1, product.ProductID);
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var list = new List<ProductListDto>
            {
                new ProductListDto
                {
                    ProductID = 1,
                    ProductName = "Test"
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