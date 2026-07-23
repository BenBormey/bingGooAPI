using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Product;
using JuJuBiAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
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



        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Search name is required" });

            var products = await _product.SearchByNameAsync(name);

            return Ok(products);
        }


        [HttpGet("search-sku")]
        public async Task<IActionResult> SearchBySku([FromQuery] string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return BadRequest(new { message = "Search sku is required" });

            var products = await _product.SearchBySkuAsync(sku);

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


        [PermissionAuthorize("PRODUCT")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)  
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

       
       

            var created = await _product.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }



        [PermissionAuthorize("PRODUCT")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.ProID)
                return BadRequest(new { message = "ID mismatch" });

            var exists = await _product.ExistsAsync(id);

            if (!exists)
                return NotFound(new { message = "Product not found" });

            var result = await _product.UpdateAsync(dto);

            if (!result)
                return BadRequest(new
                {
                    message = "Update failed"
                });

            return Ok(new
            {
                message = "Updated successfully"
            });
        }


        [PermissionAuthorize("PRODUCT")]
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
        // Images only — uploads land in wwwroot and are served publicly, so
        // never accept executable/script files here.
        private static readonly string[] AllowedImageExtensions =
            { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        private const long MaxImageBytes = 5 * 1024 * 1024; // 5 MB

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (file.Length > MaxImageBytes)
                return BadRequest(new { message = "File too large. Maximum size is 5 MB." });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedImageExtensions.Contains(extension))
                return BadRequest(new
                {
                    message = $"File type '{extension}' is not allowed. Allowed: {string.Join(", ", AllowedImageExtensions)}"
                });

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(folder, fileName);

            await using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/products/{fileName}";

            return Ok(new
            {
                imageUrl
            });
        }
        [HttpGet("barcode/{barcode}")]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            var product = await _product.GetByBarcodeAsync(barcode);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found."
                });
            }

            return Ok(product);
        }

        [PermissionAuthorize("PRODUCT")]
        [HttpPut("{id}/case-number")]
        public async Task<IActionResult> UpdateCaseNumber(int id, [FromBody] UpdateCaseNumberDto dto)
        {
            var exists = await _product.ExistsAsync(id);

            if (!exists)
                return NotFound(new { message = "Product not found" });

            var updated = await _product.UpdateCaseNumberAsync(id, dto.CaseNumber);

            if (!updated)
                return BadRequest(new { message = "Update failed" });

            return Ok(new { message = "Updated successfully" });
        }

        [PermissionAuthorize("PRODUCT")]
        [HttpPut("{id}/barcode")]
        public async Task<IActionResult> UpdateBarcode(int id, [FromBody] UpdateProductFieldDto dto)
        {
            var exists = await _product.ExistsAsync(id);

            if (!exists)
                return NotFound(new { message = "Product not found" });

            var updated = await _product.UpdateBarcodeAsync(id, dto.Value);

            if (!updated)
                return BadRequest(new { message = "Update failed" });

            return Ok(new { message = "Updated successfully" });
        }

        [PermissionAuthorize("PRODUCT")]
        [HttpPut("{id}/old-barcode")]
        public async Task<IActionResult> UpdateOldBarcode(int id, [FromBody] UpdateProductFieldDto dto)
        {
            var exists = await _product.ExistsAsync(id);

            if (!exists)
                return NotFound(new { message = "Product not found" });

            var updated = await _product.UpdateOldBarcodeAsync(id, dto.Value);

            if (!updated)
                return BadRequest(new { message = "Update failed" });

            return Ok(new { message = "Updated successfully" });
        }

        [PermissionAuthorize("PRODUCT")]
        [HttpPut("{id}/pack-number")]
        public async Task<IActionResult> UpdatePackNumber(int id, [FromBody] UpdateProductFieldDto dto)
        {
            var exists = await _product.ExistsAsync(id);

            if (!exists)
                return NotFound(new { message = "Product not found" });

            var updated = await _product.UpdatePackNumberAsync(id, dto.Value);

            if (!updated)
                return BadRequest(new { message = "Update failed" });

            return Ok(new { message = "Updated successfully" });
        }

        [PermissionAuthorize("PRODUCT")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateProductFieldDto dto)
        {
            var exists = await _product.ExistsAsync(id);

            if (!exists)
                return NotFound(new { message = "Product not found" });

            var updated = await _product.UpdateStatusAsync(id, dto.Value);

            if (!updated)
                return BadRequest(new { message = "Update failed" });

            return Ok(new { message = "Updated successfully" });
        }

    }
}
