using System.Data;
using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;

namespace JuJuBiAPI.Repositories
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly IDbConnection _connection;

        public ShiftRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Shift?> GetOpenShiftAsync(int outletId, int userId)
        {
            return await _connection.QueryFirstOrDefaultAsync<Shift>(
                ShiftQueries.GetOpenShift, new { OutletId = outletId, UserId = userId });
        }

        public async Task<Shift> OpenAsync(int outletId, int userId, decimal openingFloat)
        {
            // One open shift per cashier per outlet — opening again while one is
            // open would orphan the first and break the reconciliation chain.
            var existing = await GetOpenShiftAsync(outletId, userId);
            if (existing != null)
                throw new InvalidOperationException(
                    $"Shift #{existing.ShiftID} is already open (since {existing.OpenedAt:HH:mm}). Close it first.");

            return await _connection.QuerySingleAsync<Shift>(
                ShiftQueries.Open, new { OutletId = outletId, UserId = userId, OpeningFloat = openingFloat });
        }

        public async Task<ShiftSummary?> GetSummaryAsync(int shiftId)
        {
            var summary = await _connection.QueryFirstOrDefaultAsync<ShiftSummary>(
                ShiftQueries.GetSummary, new { ShiftID = shiftId });

            if (summary != null)
                summary.ExpectedCash = summary.OpeningFloat + summary.SalesCash;

            return summary;
        }

        public async Task<Shift?> CloseAsync(int shiftId, decimal countedCash, string notes)
        {
            var summary = await GetSummaryAsync(shiftId);
            if (summary == null)
                return null;

            return await _connection.QueryFirstOrDefaultAsync<Shift>(ShiftQueries.Close, new
            {
                ShiftID = shiftId,
                ExpectedCash = summary.ExpectedCash,
                CountedCash = countedCash,
                Notes = notes
            });
        }
    }
}
