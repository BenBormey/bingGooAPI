using JuJuBiAPI.Interfaces;
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

        public VatSettingController(IVatSettingRepository repository)
        {
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
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromQuery] decimal percent,
            [FromQuery] string? updatedBy = "system")
        {
            if (percent < 0 || percent >= 100)
                return BadRequest("Percent must be between 0 and 100.");

            var success = await _repository.UpdateAsync(percent, updatedBy);

            if (!success)
                return StatusCode(500, new { message = "Could not update the VAT rate." });

            return Ok(new { message = "VAT rate updated to " + percent + "%.", percent });
        }
    }
}
