using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly IcurrencyRepository _service;

        public CurrencyController(IcurrencyRepository service)
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
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Currency model)
        {


            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Currency model)
        {
            if (id != model.Id) return BadRequest();
            var ok = await _service.UpdateAsync(model);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
