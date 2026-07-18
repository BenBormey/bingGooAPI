using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.OutletOrder;
using Microsoft.AspNetCore.Mvc;

namespace JuJuBiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutletOrderController : ControllerBase
    {
        private readonly IOutletOrderRepository _repo;

        public OutletOrderController(IOutletOrderRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var outletOrders = await _repo.GetAllAsync();
            return Ok(outletOrders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var outletOrder = await _repo.GetByIdAsync(id);

            if (outletOrder == null)
                return NotFound("Outlet order not found");

            return Ok(outletOrder);
        }

        [HttpGet("outlet/{outletId}")]
        public async Task<IActionResult> GetByOutlet(int outletId)
        {
            var outletOrders = await _repo.GetByOutletAsync(outletId);
            return Ok(outletOrders);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOutletOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var outletOrder = await _repo.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = outletOrder.OutletOrderID },
                    outletOrder);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to create outlet order",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest("Status is required");

            var updated = await _repo.UpdateStatusAsync(id, status);

            if (!updated)
                return NotFound("Outlet order not found");

            return Ok("Status updated");
        }

        [HttpPost("fulfill/{id}")]
        public async Task<IActionResult> Fulfill(int id, [FromBody] FulfillOutletOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var fulfilled = await _repo.FulfillAsync(id, dto);

                if (!fulfilled)
                    return NotFound("Outlet order not found");

                var outletOrder = await _repo.GetByIdAsync(id);

                return Ok(new
                {
                    Message = "Outlet order fulfilled",
                    OutletOrder = outletOrder
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to fulfill outlet order",
                    Error = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var outletOrder = await _repo.GetByIdAsync(id);

            if (outletOrder == null)
                return NotFound(new { Success = false, Message = "Outlet order not found." });

            if (outletOrder.Status is "Fulfilled" or "PartiallyFulfilled")
                return BadRequest(new
                {
                    Success = false,
                    Message = "Cannot delete an outlet order that has already been fulfilled."
                });

            await _repo.DeleteAsync(id);

            return Ok(new { Success = true, Message = "Outlet order deleted successfully." });
        }
    }
}
