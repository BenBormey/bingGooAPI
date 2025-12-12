using bingGooAPI.Entities;

namespace bingGooAPI.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, string? Token, User? User)> LoginAsync(
            string username,
            string password
        );
    }
}
