using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _repository;

        public CustomerController(ICustomerRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        // GET: api/Customer/search?q=096261 — POS member lookup
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Ok(Array.Empty<Customer>());

            var result = await _repository.SearchAsync(q.Trim());
            return Ok(result);
        }

        // GET: api/Customer/next-code
        [HttpGet("next-code")]
        public async Task<IActionResult> GetNextCode()
        {
            var code = await _repository.GetNextCodeAsync();
            return Ok(new { customerCode = code });
        }

        // GET: api/Customer/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Customer not found" });

            return Ok(result);
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.CustomerCode) || string.IsNullOrWhiteSpace(customer.CustomerName))
                return BadRequest(new { message = "Customer code and name are required" });

            if (await _repository.ExistsByCodeAsync(customer.CustomerCode))
                return BadRequest(new { message = "Customer code already exists" });

            var id = await _repository.CreateAsync(customer);

            customer.CustomerID = id;

            return CreatedAtAction(nameof(GetById), new { id }, customer);
        }

        // PUT: api/Customer/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Customer customer)
        {
            if (id != customer.CustomerID)
                return BadRequest(new { message = "CustomerID does not match" });

            if (string.IsNullOrWhiteSpace(customer.CustomerCode) || string.IsNullOrWhiteSpace(customer.CustomerName))
                return BadRequest(new { message = "Customer code and name are required" });

            if (await _repository.ExistsByCodeAsync(customer.CustomerCode, id))
                return BadRequest(new { message = "Customer code already exists" });

            var updated = await _repository.UpdateAsync(customer);

            if (!updated)
                return NotFound(new { message = "Customer not found" });

            return Ok(new { message = "Updated successfully" });
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Customer not found" });

            return Ok(new { message = "Deleted successfully" });
        }
    }
}
