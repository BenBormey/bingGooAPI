using bingGooAPI.Interfaces;
using bingGooAPI.Models.ShelfLife;
using Microsoft.AspNetCore.Mvc;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShelfLifeController : ControllerBase
    {
        private readonly IShelfLifeRepository _repository;

        public ShelfLifeController(IShelfLifeRepository repository)
        {
            _repository = repository;
        }

        // GET: api/shelflife
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        // GET: api/shelflife/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Shelf life not found" });

            return Ok(result);
        }

        // POST: api/shelflife
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShelfLifeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repository.AddAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/shelflife/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShelfLifeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id)
                return BadRequest(new { message = "Id mismatch" });

            var success = await _repository.UpdateAsync(dto);

            if (!success)
                return NotFound(new { message = "Shelf life not found" });

            return Ok(new { message = "Updated successfully" });
        }

        // DELETE: api/shelflife/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);

            if (!success)
                return NotFound(new { message = "Shelf life not found" });

            return Ok(new { message = "Deleted successfully" });
        }
    }
}
