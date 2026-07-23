using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Attributes;
using JuJuBiAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    // The single outlet-wide VAT rate the POS applies to every order. Read by
    // the POS on startup; set from the management app's VAT settings form.
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VatSettingController : ControllerBase
    {
        private readonly IVatSettingRepository _repository;
        private readonly IAuditLogger _audit;

        public VatSettingController(IVatSettingRepository repository, IAuditLogger audit)
        {
            _audit = audit;
            _repository = repository;
        }

        // GET: api/VatSetting
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var setting = await _repository.GetAsync();
            return Ok(setting);
        }

        // PUT: api/VatSetting?percent=8&updatedBy=admin
        [PermissionAuthorize("VAT_SETTING")]
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromQuery] decimal percent,
            [FromQuery] string? updatedBy = "system")
        {
            if (percent < 0 || percent >= 100)
                return BadRequest("Percent must be between 0 and 100.");

            // Read the old rate first so the audit shows old -> new.
            var before = await _repository.GetAsync();

            var success = await _repository.UpdateAsync(percent, updatedBy);

            if (!success)
                return StatusCode(500, new { message = "Could not update the VAT rate." });

            await _audit.LogAsync("UPDATE", "Setting", "VatSetting", "1",
                oldValue: before.Percent.ToString("0.##") + "%",
                newValue: percent.ToString("0.##") + "%");

            return Ok(new { message = "VAT rate updated to " + percent + "%.", percent });
        }
    }
}
