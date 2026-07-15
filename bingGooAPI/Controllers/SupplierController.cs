using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Supplier;
using Microsoft.AspNetCore.Mvc;

namespace JuJuBiAPI.Controllers
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _repo.GetByIdAsync(id);

            if (supplier == null)
                return NotFound("Supplier not found");

            return Ok(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool exists = await _repo.ExistsByNameAsync(dto.SupplierName);
            if (exists)
            {
                return BadRequest(new
                {
                    Message = $"Supplier '{dto.SupplierName}' already exists."
                });
            }
            var supplier = new Supplier
            {
                SupplierCode = dto.SupplierCode,
                SupplierName = dto.SupplierName,
                ContactName = dto.ContactName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                TaxNumber = dto.TaxNumber,

                KhmerSupAddress = dto.KhmerSupAddress,
                Country = dto.Country,
                FaxLine2 = dto.FaxLine2,
                Website = dto.Website,
                LEAOTime = dto.LEAOTime,
                Note = dto.Note,
                ChequeName = dto.ChequeName,
                Term = dto.Term,
                DayOrder = dto.DayOrder,
                CountryOfPurchase = dto.CountryOfPurchase,
                SetPercentOrderLevel = dto.SetPercentOrderLevel,
                VATTEMP = dto.VATTEMP,
                SupplierNamekh = dto.SupplierNamekh,
                Status = dto.Status,
                TermId = dto.TermId
            };

            var result = await _repo.CreateAsync(supplier);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.SupplierID },
                result);
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

            existing.KhmerSupAddress = dto.KhmerSupAddress;
            existing.Country = dto.Country;
            existing.FaxLine2 = dto.FaxLine2;
            existing.Website = dto.Website;
            existing.LEAOTime = dto.LEAOTime;
            existing.Note = dto.Note;
            existing.ChequeName = dto.ChequeName;
            existing.Term = dto.Term ?? 0;
            existing.DayOrder = dto.DayOrder ?? 0;
            existing.CountryOfPurchase = dto.CountryOfPurchase;
            existing.SetPercentOrderLevel = dto.SetPercentOrderLevel;
            existing.VATTEMP = dto.VATTEMP;
            existing.SupplierNamekh = dto.SupplierNamekh;
            existing.Status = dto.Status;
            existing.TermId = dto.TermId;

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
                return NotFound(new
                {
                    Success = false,
                    Message = "Supplier not found."
                });

            await _repo.DeleteAsync(id);

            return Ok(new
            {
                Success = true,
                Message = "Supplier deleted successfully."
            });
        }
    }
}