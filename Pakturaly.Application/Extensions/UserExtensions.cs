using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Pakturaly.Data.Entities;
using Pakturaly.Shared.Utils;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Pakturaly.Application.Extensions {
    internal static class UserExtensions {
        public static RefreshToken CreateRefreshToken(this User user) {
            var utcNow = DateTime.UtcNow;

            var refreshToken = new RefreshToken {
                User = user,
                Token = CryptoHelpers.GenerateRefreshToken(),
                ExpiresAt = utcNow.AddDays(7),
                CreatedAt = utcNow
            };

            user.RefreshTokens ??= [];
            user.RefreshTokens.Add(refreshToken);

            return refreshToken;
        }

        public static (string, int) CreateAccessToken(this User user, IConfiguration config) {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(config["PrivateKey"]); //

            var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

            var userClaims = new List<Claim> {
                new(JwtRegisteredClaimNames.Sub, user.Identity.Id),
                new(JwtRegisteredClaimNames.Email, user.Identity.Email!)
            };

            ////userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var expirationMinutes = config.GetValue<int>("Jwt:ExpirationMinutes");

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow
                    .AddMinutes(expirationMinutes),
                SigningCredentials = credentials,
                Issuer = config["Jwt:Issuer"],
                Audience = config["Jwt:Audience"]
            };

            var tokenHandler = new JsonWebTokenHandler();
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);

            return (accessToken, expirationMinutes);
        }
    }
}
