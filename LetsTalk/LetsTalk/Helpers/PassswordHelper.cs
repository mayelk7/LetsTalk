using Microsoft.AspNetCore.Identity;

namespace LetsTalk.Helpers
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<object> Hasher = new();

        public static string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is empty.");

            return Hasher.HashPassword(null!, password);
        }

        public static bool Verify(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            var result = Hasher.VerifyHashedPassword(null!, hashedPassword, password);

            return result == PasswordVerificationResult.Success
                || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}