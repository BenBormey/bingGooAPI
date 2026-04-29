using Microsoft.AspNetCore.Mvc;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Outlet;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutletController : ControllerBase
    {
        private readonly IOutletRepository _outletRepository;

        public OutletController(IOutletRepository outletRepository)
        {
            _outletRepository = outletRepository;
        }

        // GET: api/outlet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OutletListDto>>> GetAll()
        {
            var outlets = await _outletRepository.GetAllAsync();
            return Ok(outlets);
        }

        // GET: api/outlet/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OutletListDto>> GetById(int id)
        {
            var outlet = await _outletRepository.GetByIdAsync(id);

            if (outlet == null)
                return NotFound(new { message = "Outlet not found" });

            return Ok(outlet);
        }

        // POST: api/outlet
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOutletDtos request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ឆែកមើលថាមាន Outlet Code នេះរួចហើយឬនៅ
            if (await _outletRepository.OutletExistsAsync(request.OutletCode))
                return BadRequest(new { message = "Outlet Code already exists" });

            var result = await _outletRepository.AddAsync(request);

            // បាញ់មកវិញនូវ Entity ដែលទើបបង្កើតរួច (រួមទាំង Id ថ្មី)
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { message = "Create Outlet complete", data = result });
        }

        // PUT: api/outlet/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOutletDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Id)
                return BadRequest(new { message = "Id mismatch" });

            // ឆែកមើលថាមានទិន្នន័យសម្រាប់ Update ឬអត់
            var success = await _outletRepository.UpdateAsync(request);

            if (!success)
                return NotFound(new { message = "Outlet not found or no changes made" });

            return Ok(new { message = "Update Outlet complete" });
        }

        // DELETE: api/outlet/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _outletRepository.DeleteAsync(id);

            if (!success)
                return NotFound(new { message = "Outlet not found" });

            return Ok(new { message = "Delete Outlet complete" });
        }
    }
}