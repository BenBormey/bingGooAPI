using JuJuBiAPI.Entities;

namespace JuJuBiAPI.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
