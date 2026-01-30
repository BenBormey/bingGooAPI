
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bingGooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        // GET: api/role
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleRepository.GetAllAsync();
            return Ok(result);
        }

        // GET: api/role/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role == null)
                return NotFound("Role not found");

            return Ok(role);
        }

        // POST: api/role
        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleDto dto)
        {
            var id = await _roleRepository.CreateAsync(dto);

            return Ok(new
            {
                message = "Role created successfully",
                roleId = id
            });
        }

        // PUT: api/role/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateRoleDto dto)
        {
            var success = await _roleRepository.UpdateAsync(id, dto);

            if (!success)
                return NotFound("Role not found");

            return Ok("Role updated successfully");
        }

        // DELETE: api/role/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _roleRepository.DeleteAsync(id);

            if (!success)
                return NotFound("Role not found");

            return Ok("Role deleted successfully");
        }
    }
}
