using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Term;
using JuJuBiAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TermDayController : ControllerBase
    {
        private readonly ITermDayRepository _repository;

        public TermDayController(ITermDayRepository repository)
        {
            _repository = repository;
        }

        // GET: api/termday
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        // GET: api/termday/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Term day not found" });

            return Ok(result);
        }

        // POST: api/termday
        [PermissionAuthorize("SUPPLIER")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTermDayDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repository.AddAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/termday/5
        [PermissionAuthorize("SUPPLIER")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTermDayDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id)
                return BadRequest(new { message = "Id mismatch" });

            var success = await _repository.UpdateAsync(dto);

            if (!success)
                return NotFound(new { message = "Term day not found" });

            return Ok(new { message = "Updated successfully" });
        }

        // DELETE: api/termday/5
        [PermissionAuthorize("SUPPLIER")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);

            if (!success)
                return NotFound(new { message = "Term day not found" });

            return Ok(new { message = "Deleted successfully" });
        }
    }
}
