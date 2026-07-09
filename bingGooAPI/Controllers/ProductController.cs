using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Product;
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
        [HttpGet("pos")]
        public async Task<IActionResult> GetForPOS([FromQuery] int? categoryId)
        {
            int outletId = int.Parse(User.FindFirst("OutletId")!.Value);

            var products = await _product.GetForPosAsync(outletId, categoryId);

            return Ok(products);
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


    }
}
