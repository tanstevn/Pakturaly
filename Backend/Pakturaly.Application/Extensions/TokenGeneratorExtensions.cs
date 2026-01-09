using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Pakturaly.Data.Entities;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Pakturaly.Application.Extensions {
    public static class TokenGeneratorExtensions {
        public static RefreshToken GenerateRefreshToken(this UserIdentity user) {
            return new RefreshToken {
                User = user.User,
                Token = Guid.NewGuid()
                    .ToString(),
                ExpiresAt = DateTime.UtcNow
                    .AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
        }

        public static string GenerateAccessToken(this UserIdentity user, IList<string> roles, IConfiguration config) {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(string.Empty); //

            var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

            var userClaims = new List<Claim> {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!)
            };

            userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var expirationMinutes = config.GetValue<int>("Jwt:ExpirationMinutes");

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow
                    .AddMinutes(expirationMinutes),
                SigningCredentials = credentials,
                Issuer = config["Jwt:Issuer"],
                Audience = config["Jwt:Audience"]
            };

            return new JsonWebTokenHandler()
                .CreateToken(tokenDescriptor);
        }
    }
}
