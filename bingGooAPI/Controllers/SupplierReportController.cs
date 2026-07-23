using JuJuBiAPI.Attributes;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reports/[controller]")]
    public class SupplierReportController : ControllerBase
    {
        private readonly ISupplierReportRepository _repository;

        public SupplierReportController(ISupplierReportRepository repository)
        {
            _repository = repository;
        }

        [PermissionAuthorize("SUPPLIER_REPORT")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SupplierReportFilter filter)
        {
            var data = await _repository.GetReportAsync(filter);
            return Ok(data);
        }
    }
}
