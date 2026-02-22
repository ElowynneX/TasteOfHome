using System.Security.Cryptography;

namespace TasteOfHome.Services
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;       // 128-bit
        private const int KeySize = 32;        // 256-bit
        private const int Iterations = 100_000;

        // Store as: base64(salt).base64(hash)
        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrWhiteSpace(stored)) return false;

            var parts = stored.Split('.', 2);
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedKey = Convert.FromBase64String(parts[1]);

            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return CryptographicOperations.FixedTimeEquals(key, storedKey);
        }
    }
}