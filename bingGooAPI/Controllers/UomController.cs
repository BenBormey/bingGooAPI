using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JuJuBiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UomController : ControllerBase
    {
        private readonly IUomRepository _repository;

        public UomController(IUomRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Uom
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        // GET: api/Uom/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST: api/Uom
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UOM uom)
        {
            if (uom == null)
                return BadRequest();

            var id = await _repository.CreateAsync(uom);

            uom.UOMId = id;

            return CreatedAtAction(nameof(GetById), new { id }, uom);
        }

        // PUT: api/Uom/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UOM uom)
        {
            if (id != uom.UOMId)
                return BadRequest("UOMId does not match.");

            var updated = await _repository.UpdateAsync(uom);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Uom/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}