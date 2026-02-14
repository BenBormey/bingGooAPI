using bingGooAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _repo;

        public ReportController(IReportRepository repo)
        {
            _repo = repo;
        }

        // =========================
        // PROFIT & LOSS
        // =========================
        [HttpGet("pnl")]
        public async Task<IActionResult> GetPnL(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var data = await _repo.GetPnLAsync(from, to);
            return Ok(data);
        }

 
        [HttpGet("balance-sheet")]
        public async Task<IActionResult> GetBalanceSheet(
            [FromQuery] DateTime asOfDate)
        {
            var data = await _repo.GetBalanceSheetAsync(asOfDate);
            return Ok(data);
        }
        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
        {
            if (from > to)
                return BadRequest("From date must be <= To date");

            var data = await _repo.GetSalesReportAsync(from, to);
            return Ok(data);
        }
    }
}
