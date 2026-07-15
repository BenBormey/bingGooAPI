using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace JuJuBiAPI.Controllers
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
                    id = result.User!.Id,
                    username = result.User.Username,
                    fullName = result.User.FullName,
                    roleName = result.User.RoleName,
                    roleCode = result.User.RoleCode,
                    outletId = result.User.OutletId
                }
            });
        }

        [HttpPost("md-login")]
        public async Task<IActionResult> MdLogin(MdLoginRequest req)
        {
            var result = await _auth.LoginMdAsync(req.Password);

            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(new
            {
                access_token = result.Token,
                token_type = "Bearer",

                user = new
                {
                    id = result.User!.Id,
                    username = result.User.Username,
                    fullName = result.User.FullName,
                    roleName = result.User.RoleName,
                    roleCode = result.User.RoleCode,
                    outletId = result.User.OutletId
                }
            });
        }

    }
}
