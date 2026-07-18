using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.ProductDeliveryLogistic;
using Microsoft.AspNetCore.Mvc;

namespace JuJuBiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDeliveryLogisticController : ControllerBase
    {
        private readonly IProductDeliveryLogisticRepository _repo;

        public ProductDeliveryLogisticController(IProductDeliveryLogisticRepository repo)
        {
            _repo = repo;
        }

        // GET: api/ProductDeliveryLogistic/product/0000000232334
        [HttpGet("product/{proNumY}")]
        public async Task<IActionResult> GetByProduct(string proNumY)
        {
            var list = await _repo.GetByProductAsync(proNumY);
            return Ok(list);
        }

        // POST: api/ProductDeliveryLogistic
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDeliveryLogisticDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _repo.CreateAsync(dto);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/ProductDeliveryLogistic/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Delivery logistic record not found" });

            return Ok(new { message = "Deleted successfully" });
        }
    }
}
