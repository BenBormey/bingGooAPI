using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FranchiseTypeController : ControllerBase
    {
        private readonly IFranchiseTypeItemRepository _service;

        // ចាក់ Interface Service ចូលតាមរយៈ Constructor Injection
        public FranchiseTypeController(IFranchiseTypeItemRepository service)
        {
            _service = service;
        }

        // ១. GET: api/FranchiseType
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        // ២. GET: api/FranchiseType/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        // ៣. POST: api/FranchiseType
        [HttpPost]
        public async Task<IActionResult> Create(FranchiseType model)
        {
            var created = await _service.CreateAsync(model);

            // ត្រឡប់មកវិញជា Status 201 Created ព្រមទាំងបង្ហាញ Link ទៅកាន់ GetById នៃ ID ថ្មីនោះ
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ៤. PUT: api/FranchiseType/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, FranchiseType model)
        {
            if (model.Id != id) return BadRequest();

            var ok = await _service.UpdateAsync(model);
            if (!ok) return NotFound();

            return NoContent(); // Status 204 (ជោគជ័យ និងគ្មានមាតិកាត្រូវបង្ហាញបន្ថែម)
        }

        // ៥. DELETE: api/FranchiseType/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();

            return NoContent();
        }
    }
}