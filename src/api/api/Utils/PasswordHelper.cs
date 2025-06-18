using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace FinSync.Utils
{
    public static class PasswordHelper
    {
        // Returns hashed password in format: {saltBase64}:{hashBase64}
        public static string HashPassword(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32);
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));
            if (providedPassword == null) throw new ArgumentNullException(nameof(providedPassword));
            var parts = hashedPassword.Split(':');
            if (parts.Length != 2) return false;
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedHash = Convert.FromBase64String(parts[1]);
            byte[] actualHash = KeyDerivation.Pbkdf2(providedPassword, salt, KeyDerivationPrf.HMACSHA256, 10000, 32);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}