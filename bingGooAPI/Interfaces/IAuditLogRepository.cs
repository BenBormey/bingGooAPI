using JuJuBiAPI.Entities;

namespace JuJuBiAPI.Interfaces
{
    public interface IAuditLogRepository
    {
        Task InsertAsync(AuditLog log);

        Task<List<AuditLog>> GetRecentAsync(int take, string? action, string? module, string? userName);
    }
}
