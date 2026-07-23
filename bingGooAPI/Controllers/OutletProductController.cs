using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models;
using JuJuBiAPI.Models.OutletProduct;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]        // -> api/OutletProduct
    public class OutletProductController : ControllerBase
    {
        private readonly IOutletProductRepository _repo;

        public OutletProductController(IOutletProductRepository repo)
        {
            _repo = repo;
        }

        // GET api/OutletProduct/by-outlet/5?search=coca
        [HttpGet("by-outlet/{outletId:int}")]
        public async Task<IActionResult> GetByOutlet(int outletId, [FromQuery] string? search)
        {
            var data = await _repo.GetByOutletAsync(outletId, search);
            return Ok(data);
        }

        // GET api/OutletProduct/sellable/5
        [HttpGet("sellable/{outletId:int}")]
        public async Task<IActionResult> GetSellable(int outletId)
        {
            var data = await _repo.GetSellableAsync(outletId);
            return Ok(data);
        }

        // POST api/OutletProduct/upsert
        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert([FromBody] OutletProductSave model)
        {
            if (string.IsNullOrWhiteSpace(model.ProNumY) || model.OutletId <= 0)
                return BadRequest("OutletId and ProNumY are required.");

            var rows = await _repo.UpsertAsync(model);
            return Ok(new { affected = rows });
        }

        // POST api/OutletProduct/bulk-upsert
        [HttpPost("bulk-upsert")]
        public async Task<IActionResult> BulkUpsert([FromBody] List<OutletProductSave> items)
        {
            if (items == null || items.Count == 0)
                return BadRequest("No items to save.");

            var rows = await _repo.BulkUpsertAsync(items);
            return Ok(new { affected = rows });
        }

        // PUT api/OutletProduct/can-sell?outletId=5&proNumY=4897..&canSell=true
        [HttpPut("can-sell")]
        public async Task<IActionResult> SetCanSell(
            [FromQuery] int outletId, [FromQuery] string proNumY, [FromQuery] bool canSell)
        {
            var ok = await _repo.SetCanSellAsync(outletId, proNumY, canSell);
            return ok ? Ok() : NotFound();
        }

        // DELETE api/OutletProduct/12
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repo.DeleteAsync(id);
            return ok ? Ok() : NotFound();
        }
    }
}
