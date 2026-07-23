using JuJuBiAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace JuJuBiAPI.Attributes
{
    /// <summary>
    /// Rejects the request with 403 unless the caller's role has the given
    /// screen permission (RolePermissions table). The MD role (ADMIN) always
    /// passes. Requires the caller to be authenticated (JWT).
    /// Usage: [PermissionAuthorize("SUPPLIER")]
    /// </summary>
    public class PermissionAuthorizeAttribute : TypeFilterAttribute
    {
        public PermissionAuthorizeAttribute(string permissionCode)
            : base(typeof(PermissionAuthorizationFilter))
        {
            Arguments = new object[] { permissionCode };
        }
    }

    public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permissionCode;
        private readonly IPermissionRepository _repo;

        public PermissionAuthorizationFilter(string permissionCode, IPermissionRepository repo)
        {
            _permissionCode = permissionCode;
            _repo = repo;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleCode = user.FindFirst(ClaimTypes.Role)?.Value;

            // MD always has full access.
            if (roleCode == "ADMIN")
                return;

            if (string.IsNullOrEmpty(roleCode) ||
                !await _repo.HasPermissionAsync(roleCode, _permissionCode))
            {
                context.Result = new ObjectResult(
                    new { message = "You do not have permission to use this function!" })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
