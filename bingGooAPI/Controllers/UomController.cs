using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UomController : ControllerBase
    {
        private readonly IUomRepository _repository;

        public UomController(IUomRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [PermissionAuthorize("PRODUCT")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UOM uom)
        {
            if (uom == null)
                return BadRequest();

            var id = await _repository.CreateAsync(uom);

            uom.UOMId = id;

            return CreatedAtAction(nameof(GetById), new { id }, uom);
        }


        [PermissionAuthorize("PRODUCT")]
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

 
        [PermissionAuthorize("PRODUCT")]
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
