using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models;
using JuJuBiAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JuJuBiAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IAuditLogger _audit;

        public AuthController(IAuthService auth, IAuditLogger audit)
        {
            _auth = auth;
            _audit = audit;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var result = await _auth.LoginAsync(req.Username, req.Password);

            if (!result.Success)
            {
                // Failed attempts are worth a trail too (wrong password, etc.).
                await _audit.LogAsync("LOGIN_FAILED", "Auth", "Users", req.Username ?? "",
                    remark: result.Message, userNameOverride: req.Username);

                return Unauthorized(result.Message);
            }

            await _audit.LogAsync("LOGIN", "Auth", "Users", result.User!.Id.ToString(),
                userIdOverride: result.User.Id, userNameOverride: result.User.Username);

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
            {
                await _audit.LogAsync("LOGIN_FAILED", "Auth", "Users", "md-login",
                    remark: result.Message);

                return Unauthorized(result.Message);
            }

            await _audit.LogAsync("LOGIN", "Auth", "Users", result.User!.Id.ToString(),
                userIdOverride: result.User.Id, userNameOverride: result.User.Username);

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
