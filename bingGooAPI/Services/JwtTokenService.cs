using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JuJuBiAPI.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("OutletId", user.OutletId?.ToString() ?? string.Empty),
          
                new Claim(ClaimTypes.Role, user.RoleCode)
            };

            var keyString = _config["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "Jwt:Key is not configured. In production set the Jwt__Key environment variable.");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyString)
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.TryParse(_config["Jwt:ExpireMinutes"], out var expireMinutes)
                        ? expireMinutes
                        : 60
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
