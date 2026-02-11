namespace LetsTalk.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Utilisateur? User { get; set; }

        // Méthodes helper
        public static AuthResult Succeeded(Utilisateur user)
        {
            return new AuthResult { Success = true, User = user };
        }

        public static AuthResult Failed(string errorMessage)
        {
            return new AuthResult { Success = false, ErrorMessage = errorMessage };
        }
    }
}
