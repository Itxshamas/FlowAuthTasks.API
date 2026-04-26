using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Linq;
using FlowAuthTasks.API.Models;

namespace FlowAuthTasks.API.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            // Add roles
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var keyBytes = GetKeyBytes(_config["Jwt:Key"]!);
            var key = new SymmetricSecurityKey(keyBytes);

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_config["Jwt:DurationInMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static byte[] GetKeyBytes(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("JWT key is not configured. Set 'Jwt:Key' in appsettings or user secrets.");

            try
            {
                var maybe = Convert.FromBase64String(key);
                if (Convert.ToBase64String(maybe) == key.Trim())
                {
                    if (maybe.Length < 32)
                        throw new InvalidOperationException("JWT key must be at least 256 bits (32 bytes). Provide a longer secret (use a base64-encoded 32+ byte value).");
                    return maybe;
                }
            }
            catch 

            var bytes = Encoding.UTF8.GetBytes(key);
            if (bytes.Length < 32)
                throw new InvalidOperationException("JWT key must be at least 256 bits (32 bytes). Provide a longer secret or use a base64-encoded secret.");

            return bytes;
        }
    }
}
