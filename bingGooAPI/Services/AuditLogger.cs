using System.Security.Claims;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;

namespace JuJuBiAPI.Services
{
    public interface IAuditLogger
    {
        // Fire-and-log: pulls user + IP from the current request. Never
        // throws — a failed audit write must not break the business action
        // it's recording. The overrides exist for anonymous requests (login)
        // where the JWT claims aren't populated yet.
        Task LogAsync(string action, string module, string tableName, string recordId,
            string? oldValue = null, string? newValue = null, string? remark = null,
            int? userIdOverride = null, string? userNameOverride = null);
    }

    public class AuditLogger : IAuditLogger
    {
        private readonly IAuditLogRepository _repo;
        private readonly IHttpContextAccessor _http;

        public AuditLogger(IAuditLogRepository repo, IHttpContextAccessor http)
        {
            _repo = repo;
            _http = http;
        }

        public async Task LogAsync(string action, string module, string tableName, string recordId,
            string? oldValue = null, string? newValue = null, string? remark = null,
            int? userIdOverride = null, string? userNameOverride = null)
        {
            try
            {
                var ctx = _http.HttpContext;
                var user = ctx?.User;

                int? userId = userIdOverride;
                if (userId == null)
                {
                    var idClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (int.TryParse(idClaim, out var parsed))
                        userId = parsed;
                }

                await _repo.InsertAsync(new AuditLog
                {
                    UserId = userId,
                    UserName = userNameOverride ?? user?.FindFirstValue(ClaimTypes.Name),
                    Action = action,
                    Module = module,
                    TableName = tableName,
                    RecordId = recordId ?? "",
                    OldValue = oldValue,
                    NewValue = newValue,
                    IPAddress = ctx?.Connection?.RemoteIpAddress?.ToString(),
                    DeviceName = ctx?.Request?.Headers["User-Agent"].ToString() is { Length: > 0 } ua
                        ? (ua.Length > 200 ? ua.Substring(0, 200) : ua)
                        : null,
                    Remark = remark
                });
            }
            catch
            {
                // Swallow — auditing is best-effort by design.
            }
        }
    }
}
