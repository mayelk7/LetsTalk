namespace LetsTalk.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Utilisateur? User { get; set; }

        public bool Requires2FA { get; set; }
        public string? Token { get; set; }

        public static AuthResult Succeeded(Utilisateur user)
        {
            return new AuthResult
            {
                Success = true,
                User = user,
                Requires2FA = false,
                Token = null
            };
        }

        public static AuthResult Failed(string errorMessage)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                User = null,
                Requires2FA = false,
                Token = null
            };
        }
    }
}