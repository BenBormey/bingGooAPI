using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Report;
using Microsoft.AspNetCore.Mvc;

namespace JuJuBiAPI.Controllers
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