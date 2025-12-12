using Microsoft.AspNetCore.Mvc;
using bingGooAPI.Interfaces;
using bingGooAPI.Entities;
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
        public async Task<IActionResult> GetAll()
        {
            var outlets = await _outletRepository.GetAllAsync();
            return Ok(outlets);
        }

        // GET: api/outlet/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var outlet = await _outletRepository.GetByIdAsync(id);
            if (outlet == null)
                return NotFound("Outlet not found");

            return Ok(outlet);
        }

        // POST: api/outlet
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OutletRequest request)
        {
            // check duplicate code
            var existing = await _outletRepository.GetByCodeAsync(request.OutletCode);
            if (existing != null)
                return BadRequest("Outlet code already exists");

            var outlet = new Outlet
            {
                OutletCode = request.OutletCode,
                OutletName = request.OutletName,
                Province = request.Province,
                Phone = request.Phone,
                Manager = request.Manager,
                Address = request.Address,
                HeadOffice = request.HeadOffice
            };

            await _outletRepository.AddAsync(outlet);
            await _outletRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = outlet.Id }, outlet);
        }

        // PUT: api/outlet/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] OutletRequest request)
        {
            var outlet = await _outletRepository.GetByIdAsync(id);
            if (outlet == null)
                return NotFound("Outlet not found");

            outlet.OutletCode = request.OutletCode;
            outlet.OutletName = request.OutletName;
            outlet.Province = request.Province;
            outlet.Phone = request.Phone;
            outlet.Manager = request.Manager;
            outlet.Address = request.Address;
            outlet.HeadOffice = request.HeadOffice;

            await _outletRepository.UpdateAsync(outlet);
            await _outletRepository.SaveChangesAsync();

            return Ok(outlet);
        }

        // DELETE: api/outlet/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var outlet = await _outletRepository.GetByIdAsync(id);
            if (outlet == null)
                return NotFound("Outlet not found");

            await _outletRepository.DeleteAsync(outlet);
            await _outletRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
