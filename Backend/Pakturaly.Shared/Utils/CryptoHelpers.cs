using System.Security.Cryptography;

namespace Pakturaly.Shared.Utils {
    public static class CryptoHelpers {
        public static string GenerateRefreshToken() {
            var randomBytes = new byte[32];

            using var numberGenerator = RandomNumberGenerator.Create();
            numberGenerator.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }
    }
}
