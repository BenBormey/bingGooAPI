using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.HouseOpration;
using Microsoft.AspNetCore.Mvc;

namespace JuJuBiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HourOperationController : ControllerBase
    {
        private readonly IHourOperationRepository _repository;

        public HourOperationController(IHourOperationRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHourOperationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repository.AddAsync(dto);

            return Ok(result);
        }

              [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateHourOperationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _repository.UpdateAsync(dto);

            if (!success)
                return NotFound();

            return Ok(new
            {
                Message = "Updated Successfully."
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);

            if (!success)
                return NotFound();

            return Ok(new
            {
                Message = "Deleted Successfully."
            });
        }
    }
}