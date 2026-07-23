using JuJuBiAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JuJuBiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _repo;

        public PermissionController(IPermissionRepository repo)
        {
            _repo = repo;
        }

        // GET: api/permission  (list all permissions - MD only)
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repo.GetAllAsync();
            return Ok(result);
        }

        // GET: api/permission/role/5  (permission ids granted to a role - MD only)
        [Authorize(Roles = "ADMIN")]
        [HttpGet("role/{roleId:int}")]
        public async Task<IActionResult> GetRolePermissions(int roleId)
        {
            var result = await _repo.GetRolePermissionIdsAsync(roleId);
            return Ok(result);
        }

        // PUT: api/permission/role/5  (replace a role's permissions - MD only)
        [Authorize(Roles = "ADMIN")]
        [HttpPut("role/{roleId:int}")]
        public async Task<IActionResult> SaveRolePermissions(int roleId, [FromBody] List<int> permissionIds)
        {
            await _repo.SaveRolePermissionsAsync(roleId, permissionIds ?? new List<int>());
            return Ok(new { message = "Permissions saved" });
        }

        // GET: api/permission/my  (permission codes of the logged-in user's role)
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyPermissions()
        {
            var roleCode = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(roleCode))
                return Ok(new List<string>());

            var codes = await _repo.GetPermissionCodesByRoleCodeAsync(roleCode);
            return Ok(codes);
        }
    }
}
