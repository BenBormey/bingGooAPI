using Microsoft.AspNetCore.Mvc;
using bingGooAPI.Interfaces;
using bingGooAPI.Entities;
using System.Threading.Tasks;

namespace bingGooAPI.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IcategoryRepository _service;

        public CategoryController(IcategoryRepository service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null)
                return NotFound();

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Category model)
        {
            if (model.Id != id)
                return BadRequest("Id mismatch");

            var updated = await _service.UpdateAsync(model);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
