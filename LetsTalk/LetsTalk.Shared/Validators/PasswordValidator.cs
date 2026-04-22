using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace LetsTalk.Shared.Validators
{
    
      

        public static class PasswordValidator
        {
            private static readonly HashSet<string> CommonPasswords = new(StringComparer.OrdinalIgnoreCase)
            {
                "password", "azerty", "qwerty", "123456", "12345678", "000000", "111111",
                "admin", "welcome"
            };

            public static PasswordValidationResult Validate(string? password, string? username = null, string? email = null)
            {
                var result = new PasswordValidationResult();

                if (string.IsNullOrWhiteSpace(password))
                {
                    result.Errors.Add("Mot de passe vide.");
                    return result;
                }

                if (password.Length < 12) result.Errors.Add("Minimum 12 caractères.");
                if (!Regex.IsMatch(password, "[a-z]")) result.Errors.Add("Ajoute au moins 1 minuscule.");
                if (!Regex.IsMatch(password, "[A-Z]")) result.Errors.Add("Ajoute au moins 1 majuscule.");
                if (!Regex.IsMatch(password, "[0-9]")) result.Errors.Add("Ajoute au moins 1 chiffre.");
                if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]")) result.Errors.Add("Ajoute au moins 1 symbole.");
                if (password.Contains(' ')) result.Errors.Add("Évite les espaces.");
                if (CommonPasswords.Contains(password)) result.Errors.Add("Mot de passe trop courant.");
                return result;
            }
        }  
         public sealed class PasswordValidationResult
        {
            public bool IsValid => Errors.Count == 0;
            public List<string> Errors { get; } = new();
        }
    }
