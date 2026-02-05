using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Supplier;
using Microsoft.AspNetCore.Mvc;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierRepository _repo;

        public SupplierController(ISupplierRepository repo)
        {
            _repo = repo;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _repo.GetAllAsync();

            return Ok(suppliers);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _repo.GetByIdAsync(id);

            if (supplier == null)
                return NotFound("Supplier not found");

            return Ok(supplier);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var supplier = new Supplier
            {
                SupplierCode = dto.SupplierCode,
                SupplierName = dto.SupplierName,
                ContactName = dto.ContactName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                TaxNumber = dto.TaxNumber,
                Status = true
            };

            var result = await _repo.CreateAsync(supplier);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.SupplierID },
                result
            );
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _repo.GetByIdAsync(id);

            if (existing == null)
                return NotFound("Supplier not found");

            existing.SupplierCode = dto.SupplierCode;
            existing.SupplierName = dto.SupplierName;
            existing.ContactName = dto.ContactName;
            existing.Phone = dto.Phone;
            existing.Email = dto.Email;
            existing.Address = dto.Address;
            existing.TaxNumber = dto.TaxNumber;
            existing.Status = dto.Status;

            var success = await _repo.UpdateAsync(existing);

            if (!success)
                return BadRequest("Update failed");

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);

            if (existing == null)
                return NotFound("Supplier not found");

            var success = await _repo.DeleteAsync(id);

            if (!success)
                return BadRequest("Delete failed");

            return Ok("Deleted successfully");
        }
    }
}
