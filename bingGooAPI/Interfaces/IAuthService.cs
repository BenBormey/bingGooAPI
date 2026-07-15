using JuJuBiAPI.Entities;

namespace JuJuBiAPI.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, string? Token, User? User)> LoginAsync(
            string username,
            string password
        );

        Task<(bool Success, string Message, string? Token, User? User)> LoginMdAsync(
            string password
        );
    }
}
