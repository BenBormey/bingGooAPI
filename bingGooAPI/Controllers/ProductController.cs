using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Product;
using bingGooAPI.Models.Product.bingGooAPI.Models.Product;
using Microsoft.AspNetCore.Mvc;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _product;

        public ProductController(IProductRepository product)
        {
            _product = product;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _product.GetAllAsync();

            return Ok(products);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _product.GetByIdAsync(id);

            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

       
            var product = new Product
            {
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                BrandID = dto.BrandId,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                ImageUrl = dto.ImageUrl,
                CostPrice = dto.CostPrice,
                SellingPrice = dto.SellingPrice,
                DiscountPercent = dto.DiscountPercent,
                DiscountAmount = dto.DiscountAmount,
                TaxPercent = dto.TaxPercent,
                Status = dto.Status
            };

            var created = await _product.CreateAsync(product);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.ProductID },
                created
            );
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.ProductID)
                return BadRequest(new { message = "ID mismatch" });

            var exists = await _product.ExistsAsync(id);

            if (!exists)
                return NotFound(new { message = "Product not found" });

            // DTO → Entity Mapping
            var product = new Product
            {
                ProductID = dto.ProductID,
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                BrandID = dto.BrandId,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                ImageUrl = dto.ImageUrl,
                CostPrice = dto.CostPrice,
                SellingPrice = dto.SellingPrice,
                DiscountPercent = dto.DiscountPercent,
                DiscountAmount = dto.DiscountAmount,
                TaxPercent = dto.TaxPercent,
                Status = dto.Status
            };

            var result = await _product.UpdateAsync(product);

            if (!result)
                return BadRequest(new { message = "Update failed" });

            return Ok(new { message = "Updated successfully" });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _product.ExistsAsync(id);

            if (!exists)
                return NotFound(new { message = "Product not found" });

            var result = await _product.DeleteAsync(id);

            if (!result)
                return BadRequest(new { message = "Delete failed" });

            return Ok(new { message = "Deleted successfully" });
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var folder = Path.Combine("wwwroot", "uploads", "products");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            // Build Full URL
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var imageUrl = $"{baseUrl}/uploads/products/{fileName}";

            return Ok(new
            {
                imageUrl = imageUrl
            });
        }


    }
}
