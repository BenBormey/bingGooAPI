using bingGooAPI.Interfaces;
using bingGooAPI.Models;
using bingGooAPI.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bingGooAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }



       [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _service.GetByIdAsync(id);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
        {
            var result = await _service.CreateAsync(req);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message });
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto req)
        {
            var result = await _service.UpdateAsync(id, req);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);

            if (!ok)
                return NotFound("User not found");

            return Ok(new { message = "User deleted" });
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _service
                .ChangePasswordAsync(userId, dto.OldPassword, dto.NewPassword);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message });
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(
            int id,
            [FromBody] ResetPasswordDto dto)
        {
            var result = await _service
                .ResetPasswordAsync(id, dto.NewPassword);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message });
        }
    }
}
