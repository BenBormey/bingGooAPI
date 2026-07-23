using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBis.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemRepository _repository;

        public MenuItemController(IMenuItemRepository repository)
        {
            _repository = repository;
        }

        // GET: api/MenuItem
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return Ok(data);
        }

        // GET: api/MenuItem/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // GET: api/MenuItem/outlet/1
        [HttpGet("outlet/{outletId:int}")]
        public async Task<IActionResult> GetByOutlet(int outletId)
        {
            var data = await _repository.GetByOutletAsync(outletId);
            return Ok(data);
        }

        // GET: api/MenuItem/outlet/1/product/C001
        [HttpGet("outlet/{outletId:int}/product/{proNumY}")]
        public async Task<IActionResult> GetByOutletAndProduct(int outletId, string proNumY)
        {
            var item = await _repository.GetByOutletAndProductAsync(outletId, proNumY);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/MenuItem
        [HttpPost]
        public async Task<IActionResult> Create(MenuItem model)
        {
            var exists = await _repository.ExistsAsync(model.OutletId, model.ProNumY);

            if (exists)
                return BadRequest("This product already exists in the selected outlet.");

            var item = await _repository.CreateAsync(model);

            return CreatedAtAction(
                nameof(GetById),
                new { id = item.MenuItemId },
                item);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, MenuItem model)
        {
            if (id != model.MenuItemId)
                return BadRequest("MenuItemId does not match.");

            var success = await _repository.UpdateAsync(model);

            if (!success)
                return NotFound();

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

 
        // PATCH: api/MenuItem/outlet/1/discount?percent=15
        // Applies one promotion across every product in the outlet's menu.
        [HttpPatch("outlet/{outletId:int}/discount")]
        public async Task<IActionResult> SetOutletDiscount(
            int outletId,
            [FromQuery] decimal percent,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string updatedBy = "system")
        {
            if (percent <= 0 || percent >= 100)
                return BadRequest("Percent must be between 0 and 100.");

            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                return BadRequest("Start date must be on or before the end date.");

            var affected = await _repository.SetOutletDiscountAsync(
                outletId, percent, startDate, endDate, updatedBy);

            return Ok(new
            {
                Message = percent + "% discount applied to " + affected + " menu item(s).",
                Affected = affected
            });
        }

        // DELETE: api/MenuItem/outlet/1/discount — ends the promotion.
        [HttpDelete("outlet/{outletId:int}/discount")]
        public async Task<IActionResult> ClearOutletDiscount(
            int outletId,
            [FromQuery] string updatedBy = "system")
        {
            var affected = await _repository.ClearOutletDiscountAsync(outletId, updatedBy);

            return Ok(new
            {
                Message = "Discount cleared on " + affected + " menu item(s).",
                Affected = affected
            });
        }

        [HttpPatch("{id:int}/active")]
        public async Task<IActionResult> SetActive(
            int id,
            [FromQuery] bool isActive,
            [FromQuery] string updatedBy)
        {
            var success = await _repository.SetActiveAsync(id, isActive, updatedBy);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
