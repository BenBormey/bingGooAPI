using bingGooAPI.Interfaces;
using bingGooAPI.Models.Report;
using Microsoft.AspNetCore.Mvc;

namespace bingGooAPI.Controllers
{
    [ApiController]
    [Route("api/reports/[controller]")]
    public class SupplierReportController : ControllerBase
    {
        private readonly ISupplierReportRepository _repository;

        public SupplierReportController(ISupplierReportRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SupplierReportFilter filter)
        {
            var data = await _repository.GetReportAsync(filter);
            return Ok(data);
        }
    }
}