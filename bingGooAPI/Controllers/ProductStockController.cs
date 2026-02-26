using bingGooAPI.Interfaces;
using bingGooAPI.Models.ProductStock;
using Microsoft.AspNetCore.Mvc;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStockController : ControllerBase
    {
        private readonly IProductStockRepository _service;

        public ProductStockController(IProductStockRepository service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductStockDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.CreateAsync(dto);

            return Ok(new
            {
                Message = "Product stock created successfully",
                StockID = id
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();

            return Ok(data);
        }

      
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
                return NotFound(new { Message = "Stock not found" });

            return Ok(data);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetByProductBranchOutlet(
            int productId,
            int branchId,
            int outletId)
        {
            var data = await _service
                .GetByProductBranchOutletAsync(productId, branchId, outletId);

            if (data == null)
                return NotFound(new { Message = "Stock not found" });

            return Ok(data);
        }

       
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateProductStockDto dto)
        {
            if (id != dto.StockID)
                return BadRequest("ID mismatch");

            var success = await _service.UpdateAsync(dto);

            if (!success)
                return NotFound(new { Message = "Update failed" });

            return Ok(new
            {
                Message = "Product stock updated successfully"
            });
        }

 
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);

            if (!success)
                return NotFound(new { Message = "Delete failed" });

            return Ok(new
            {
                Message = "Product stock deleted successfully"
            });
        }
    }
}
