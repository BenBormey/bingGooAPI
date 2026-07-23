using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace JuJuBiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IConfiguration config, ILogger<HealthController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpGet("db")]
        public IActionResult CheckDatabase()
        {
            try
            {
                var connectionString = _config.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return Ok("✅ DefaultConnection Connected OK");
                }
            }
            catch (Exception ex)
            {
                // This endpoint is anonymous; raw SqlException text exposes
                // server names/network detail, so log it and answer generically.
                _logger.LogError(ex, "Database health check failed");
                return StatusCode(500, "❌ Database connection failed");
            }
        }
    }
}
