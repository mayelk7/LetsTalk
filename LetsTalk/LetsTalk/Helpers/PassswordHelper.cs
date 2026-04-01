using LetsTalk.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace LetsTalk.Helpers
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<object> Hasher = new();

        public static string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is empty.");

            if (password.Contains(" "))
                throw new ArgumentException("Le mot de passe ne doit pas contenir d'espace.");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                throw new ArgumentException("Le mot de passe doit contenir au moins une majuscule");

            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]"))
                throw new ArgumentException("Le mot de passe doit contenir au moins un caractère spécial");

            if (password.Length < 8)
                throw new ArgumentException("Le mot de passe doit contenir au moins 8 caractères");

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