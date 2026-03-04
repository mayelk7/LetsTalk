using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace LetsTalk.Shared.Validators
{
    public static class EmailValidator
    {
        public static bool IsValid(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            email = email.Trim();

            try
            {
                var addr = new MailAddress(email);
                return addr.Address.Equals(email, StringComparison.OrdinalIgnoreCase)
                       && addr.Host.Contains('.');
            }
            catch
            {
                return false;
            }
        }
    }
}
