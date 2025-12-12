using bingGooAPI.Interfaces;
using bingGooAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace bingGooAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var result = await _auth.LoginAsync(req.Username, req.Password);

            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(new
            {
                access_token = result.Token,
                token_type = "Bearer",
                user = new
                {
                    result.User!.Id,
                    result.User.Username,
                    result.User.FullName,
                    result.User.Role
                }
            });
        }
    }
}
