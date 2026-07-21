using JuJuBiAPI.Entities;

namespace JuJuBiAPI.Interfaces
{
    public interface IShiftRepository
    {
        Task<Shift?> GetOpenShiftAsync(int outletId, int userId);

        Task<Shift> OpenAsync(int outletId, int userId, decimal openingFloat);

        Task<ShiftSummary?> GetSummaryAsync(int shiftId);

        Task<Shift?> CloseAsync(int shiftId, decimal countedCash, string notes);
    }
}
