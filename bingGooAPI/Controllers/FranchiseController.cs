using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JuJuBiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FranchiseController : ControllerBase
    {
        private readonly IFranchiseRepository _repository;

        public FranchiseController(IFranchiseRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Franchise
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        // GET: api/Franchise/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST: api/Franchise
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Franchise model)
        {
            if (model == null)
                return BadRequest();

            var result = await _repository.InsertAsync(model);

            if (result > 0)
                return Ok(new { message = "Created successfully" });

            return BadRequest("Insert failed");
        }


       
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Franchise model)
        {
            if (model == null || model.FranchiseId != id) // ផ្ទៀងផ្ទាត់ ID ឱ្យត្រូវគ្នា
                return BadRequest("ID Mismatch");

            var result = await _repository.UpdateAsync(model);

            if (result > 0)
                return Ok(new { message = "Updated successfully" });

            return BadRequest("Update failed");
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repository.DeleteAsync(id);

            if (result > 0)
                return Ok(new { message = "Deleted successfully" });

            return NotFound();
        }

  
    }
}