using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public HealthController(IConfiguration config)
        {
            _config = config;
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
                return StatusCode(500, "❌ ERROR: " + ex.Message);
            }
        }
    }
}
