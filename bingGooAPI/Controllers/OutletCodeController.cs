using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Outlet;
using JuJuBiAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OutletCodeController : ControllerBase
    {
        private readonly IOutletCodeRepository _repository;

        public OutletCodeController(IOutletCodeRepository repository)
        {
            _repository = repository;
        }

        // GET: api/outletcode
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        // GET: api/outletcode/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Outlet code not found" });

            return Ok(result);
        }

        // GET: api/outletcode/next-code
        [HttpGet("next-code")]
        public async Task<IActionResult> GetNextCode()
        {
            var code = await _repository.GetNextCodeAsync();
            return Ok(new { outletCode = code });
        }

        // POST: api/outletcode
        [PermissionAuthorize("OUTLET")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOutletCodeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _repository.ExistsAsync(dto.OutletCode))
                return BadRequest(new { message = "Outlet Code already exists" });

            var result = await _repository.AddAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/outletcode/5
        [PermissionAuthorize("OUTLET")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOutletCodeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id)
                return BadRequest(new { message = "Id mismatch" });

            if (await _repository.ExistsAsync(dto.OutletCode, dto.Id))
                return BadRequest(new { message = "Outlet Code already exists" });

            var success = await _repository.UpdateAsync(dto);

            if (!success)
                return NotFound(new { message = "Outlet code not found" });

            return Ok(new { message = "Updated successfully" });
        }

        // DELETE: api/outletcode/5
        [PermissionAuthorize("OUTLET")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);

            if (!success)
                return NotFound(new { message = "Outlet code not found" });

            return Ok(new { message = "Deleted successfully" });
        }
    }
}
