using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;
using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class VatSettingRepository : IVatSettingRepository
    {
        private readonly IDbConnection _connection;

        public VatSettingRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<VatSetting> GetAsync()
        {
            return await _connection.QuerySingleAsync<VatSetting>(VatSettingQueries.Get);
        }

        public async Task<bool> UpdateAsync(decimal percent, string? updatedBy)
        {
            var rows = await _connection.ExecuteAsync(
                VatSettingQueries.Update,
                new { Percent = percent, UpdatedBy = updatedBy });

            return rows > 0;
        }
    }
}
