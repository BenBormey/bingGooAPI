using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using JuJuBiAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BankSetupController : ControllerBase
    {
        private readonly IBankSetupRepository _repository;

        public BankSetupController(IBankSetupRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }
        [HttpGet("active")]

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var bank = await _repository.GetByIdAsync(id);

            if (bank == null)
                return NotFound();

            return Ok(bank);
        }

        [PermissionAuthorize("BANK_MENU")]
        [HttpPost]
        public async Task<IActionResult> Create(BankSetup bank)
        {
            var id = await _repository.CreateAsync(bank);

            bank.BankId = id;

            return Ok(bank);
        }

        [PermissionAuthorize("BANK_MENU")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BankSetup bank)
        {
            bank.BankId = id;

            var result = await _repository.UpdateAsync(bank);

            if (!result)
                return NotFound();

            return Ok();
        }

        [PermissionAuthorize("BANK_MENU")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repository.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
