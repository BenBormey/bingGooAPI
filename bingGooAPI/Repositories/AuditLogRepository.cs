using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;
using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly IDbConnection _connection;

        public AuditLogRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task InsertAsync(AuditLog log)
        {
            await _connection.ExecuteAsync(AuditLogQueries.Insert, log);
        }

        public async Task<List<AuditLog>> GetRecentAsync(int take, string? action, string? module, string? userName)
        {
            var rows = await _connection.QueryAsync<AuditLog>(AuditLogQueries.GetRecent, new
            {
                Take = Math.Clamp(take, 1, 500),
                Action = action ?? "",
                Module = module ?? "",
                UserName = userName ?? ""
            });

            return rows.ToList();
        }
    }
}
