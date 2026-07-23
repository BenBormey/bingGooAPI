using JuJuBiAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    // Read-only view of the audit trail. Writes happen through IAuditLogger
    // inside the audited actions themselves — there is deliberately no POST.
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogRepository _repo;

        public AuditLogController(IAuditLogRepository repo)
        {
            _repo = repo;
        }

        // GET: api/AuditLog?take=100&action=LOGIN&module=POS&userName=admin
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int take = 100,
            [FromQuery] string? action = null,
            [FromQuery] string? module = null,
            [FromQuery] string? userName = null)
        {
            var rows = await _repo.GetRecentAsync(take, action, module, userName);
            return Ok(rows);
        }
    }
}
