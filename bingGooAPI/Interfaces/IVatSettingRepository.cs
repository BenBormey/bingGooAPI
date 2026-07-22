using JuJuBiAPI.Entities;

namespace JuJuBiAPI.Interfaces
{
    public interface IVatSettingRepository
    {
        // Always returns a row — seeds a 0% default the first time if the
        // single settings row doesn't exist yet.
        Task<VatSetting> GetAsync();

        Task<bool> UpdateAsync(decimal percent, string? updatedBy);
    }
}
