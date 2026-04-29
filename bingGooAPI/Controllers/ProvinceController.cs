using Microsoft.AspNetCore.Mvc;
using bingGooAPI.Interfaces;
using bingGooAPI.Entities;
using System.Threading.Tasks;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private readonly IProvincesRepository _service;

        public ProvinceController(IProvincesRepository service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllProvincesAsync();
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetProvinceByIdAsync(id);
            if (data == null) return NotFound(new { message = "រកមិនឃើញខេត្តដែលអ្នកចង់រកទេ" });

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Provinces model)
        {
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.ProvinceId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Provinces model)
        {
            if (model.ProvinceId != id) return BadRequest();

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