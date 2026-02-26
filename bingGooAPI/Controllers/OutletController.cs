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
        public async Task<ActionResult<List<OutletListDto>>> GetAll()
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
                return NotFound("Outlet not found");

            return Ok(outlet);
        }

   
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOutletDtos request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

          await _outletRepository.AddAsync(request);

            return Ok(new {message ="Create Outlet complete"});
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOutletDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Id)
                return BadRequest("Id mismatch");

        await _outletRepository.UpdateAsync(request);

            return Ok(new { message = "Update Outlet complete" });
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
           await _outletRepository.DeleteAsync(id);



            return Ok(new { message = "Delete Outlet complete" });

        }
    }
}
