using bingGooAPI.Entities;

namespace bingGooAPI.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
