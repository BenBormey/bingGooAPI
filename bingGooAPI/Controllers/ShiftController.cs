using JuJuBiAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftRepository _repository;

        public ShiftController(IShiftRepository repository)
        {
            _repository = repository;
        }

        public class OpenShiftRequest
        {
            public int OutletId { get; set; }
            public int UserId { get; set; }
            public decimal OpeningFloat { get; set; }
        }

        public class CloseShiftRequest
        {
            public decimal CountedCash { get; set; }
            public string? Notes { get; set; }
        }

        // GET: api/Shift/current?outletId=1&userId=2 — the cashier's open shift
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent([FromQuery] int outletId, [FromQuery] int userId)
        {
            var shift = await _repository.GetOpenShiftAsync(outletId, userId);
            return Ok(shift);
        }

        // POST: api/Shift/open
        [HttpPost("open")]
        public async Task<IActionResult> Open(OpenShiftRequest request)
        {
            if (request.OutletId <= 0 || request.UserId <= 0)
                return BadRequest("OutletId and UserId are required.");

            if (request.OpeningFloat < 0)
                return BadRequest("Opening float cannot be negative.");

            try
            {
                var shift = await _repository.OpenAsync(request.OutletId, request.UserId, request.OpeningFloat);
                return Ok(shift);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Shift/5/summary — totals for the close screen / Z-report
        [HttpGet("{shiftId:int}/summary")]
        public async Task<IActionResult> Summary(int shiftId)
        {
            var summary = await _repository.GetSummaryAsync(shiftId);

            if (summary == null)
                return NotFound("Shift not found.");

            return Ok(summary);
        }

        // POST: api/Shift/5/close
        [HttpPost("{shiftId:int}/close")]
        public async Task<IActionResult> Close(int shiftId, CloseShiftRequest request)
        {
            if (request.CountedCash < 0)
                return BadRequest("Counted cash cannot be negative.");

            var shift = await _repository.CloseAsync(shiftId, request.CountedCash, request.Notes ?? "");

            if (shift == null)
                return NotFound("Shift not found.");

            if (shift.Status != "Closed")
                return BadRequest("Shift was already closed.");

            return Ok(shift);
        }
    }
}
