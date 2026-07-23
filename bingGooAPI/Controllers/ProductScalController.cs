using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Services;
using JuJuBiAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductScaleController : ControllerBase
    {
        private readonly IProductScalRepository _repository;

        public ProductScaleController(IProductScalRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return Ok(data);
        }

        // Search by ProNumY (partial match, returns list)
        // GET api/ProductScale/search?proNumY=12345
        [HttpGet("search")]
        public async Task<IActionResult> SearchByProNumY([FromQuery] string proNumY)
        {
            if (string.IsNullOrWhiteSpace(proNumY))
                return BadRequest(new { Message = "ProNumY is required." });

            var data = await _repository.SearchByProNumYAsync(proNumY);
            return Ok(data);
        }

        // Exact barcode lookup (single record)
        // GET api/ProductScale/barcode/0000000000004
        [HttpGet("barcode/{proNumY}")]
        public async Task<IActionResult> GetByProNumY(string proNumY)
        {
            if (string.IsNullOrWhiteSpace(proNumY))
                return BadRequest(new { Message = "ProNumY is required." });

            var data = await _repository.GetByProNumYAsync(proNumY);
            if (data == null)
                return NotFound(new { Message = "Product not found for this barcode." });

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data == null)
                return NotFound();

            return Ok(data);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(decimal productId)
        {
            var data = await _repository.GetByProductIdAsync(productId);
            if (data == null)
                return NotFound();

            return Ok(data);
        }

        [PermissionAuthorize("PRODUCT")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductsScale productScale)
        {
            if (string.IsNullOrWhiteSpace(productScale.ProNumY))
                return BadRequest(new { Message = "ProNumY is required." });

            var id = await _repository.CreateAsync(productScale);

            if (id == 0)
            {
                // duplicate: same ProNumY + same UOMCode already exists
                return Conflict(new
                {
                    Message = $"This UOM '{productScale.UOMCode}' already exists for product '{productScale.ProNumY}'."
                });
            }

            return Ok(new
            {
                Message = "Created Successfully",
                Id = id
            });
        }

        [PermissionAuthorize("PRODUCT")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(decimal id, [FromBody] ProductsScale productScale)
        {
            productScale.Id = id;

            // check duplicate before update (excluding this record itself)
            var duplicate = await _repository.ExistsAsync(
                productScale.ProNumY,
                productScale.UOMCode,
                id);

            if (duplicate)
            {
                return Conflict(new
                {
                    Message = $"This UOM '{productScale.UOMCode}' already exists for product '{productScale.ProNumY}'."
                });
            }

            var result = await _repository.UpdateAsync(productScale);
            if (!result)
                return NotFound();

            return Ok(new
            {
                Message = "Updated Successfully"
            });
        }

        [PermissionAuthorize("PRODUCT")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            var result = await _repository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return Ok(new
            {
                Message = "Deleted Successfully"
            });
        }
    }
}
