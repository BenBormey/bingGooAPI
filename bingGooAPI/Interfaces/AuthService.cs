using bingGooAPI.Entities;
using bingGooAPI.Interfaces;

namespace bingGooAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IJwtTokenService _jwt;

        public AuthService(
            IUserRepository users,
            IJwtTokenService jwt)
        {
            _users = users;
            _jwt = jwt;
        }

        public async Task<(bool Success, string Message, string? Token, User? User)>
            LoginAsync(string username, string password)
        {
            var user = await _users.GetByUsernameAsync(username);

            if (user == null)
                return (false, "Invalid username or password", null, null);

            if (!user.IsActive)
                return (false, "User is inactive", null, null);

          
            if (user.PasswordHash != password)
                return (false, "Invalid username or password", null, null);

            await _users.UpdateLastLoginAsync(user.Id);

            var token = _jwt.GenerateToken(user);

            return (true, "Login success", token, user);
        }
    }
}
