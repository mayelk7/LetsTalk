using LetsTalk.Data;
using LetsTalk.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LetsTalk.Services.Authentification
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        // 2. Constructeur avec injection de dépendances
        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }
        public string HashPassword(string password)

        {
            string Hash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
            return Hash;
        }
        public AuthResult Login(string username, string password)
        {   
            // Récupération des informations de l'utilisateur
            var user = _context.Utilisateurs.FirstOrDefault(u => u.Username == username);

            // Vérifie si l'utilisateur éxiste
            if (user == null)
                return AuthResult.Failed("Nom d'utilisateur ou mot de passe incorrecte");

            // Vérifier si actif
            if (!user.Actif)
                return AuthResult.Failed("Votre compte a été désactivé");

            // Vérifie le mot de passe
            bool isMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isMatch)
            {
                return AuthResult.Failed("Nom d'utilisateur ou mot de passe incorrecte");
            }

            return AuthResult.Succeeded(user);

        }
        public AuthResult Register(string Username, string Email, string phone, string password)
        {
            // Vérification du username
            if (Username.Length > 255)
            {
                return AuthResult.Failed("Nom d'utilisateur trop long");
            }
            var existingUser = _context.Utilisateurs.FirstOrDefault(u => u.Username == Username);
            if (existingUser != null)
                return AuthResult.Failed("Ce nom d'utilisateur est déjà utilisé");

            // Vérification de l'email
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return AuthResult.Failed("Format d'email incorrecte");
            }

            var existingEmail = _context.Utilisateurs.FirstOrDefault(u => u.Email == Email);
            if (existingEmail != null)
                return AuthResult.Failed("Cet email est déja utilisé");

            // Validation de la taille du mot de passe
            if (password.Contains(" "))
            {
                return AuthResult.Failed("Attention votre mot de passe contient un espace");
            }

            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]"))
            {
                return AuthResult.Failed("Le Mot de passe doit contenir au moins 1 caractère spécial");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return AuthResult.Failed("Le Mot de passe doit contenir au moins 1 majuscule");
            }

            if (password.Length < 8)
            return AuthResult.Failed("Le mot de passe doit contenir au moins 8 caractères");

            // Hash du mot de passe
            string HashPasswd = HashPassword(password);

            // Création du nouvel utilisateur et ajout de celui ci à la DB
            Utilisateur newUser = new Utilisateur(Username, Email, phone, HashPasswd);

            try
            {
                _context.Utilisateurs.Add(newUser);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return AuthResult.Failed("Ce nom d'utilisateur ou cet email est déjà utilisé");
            }

            return AuthResult.Succeeded(newUser);
        }
    }
}
