using LetsTalk.Context;
using LetsTalk.Helpers;
using LetsTalk.Models;
using LetsTalk.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LetsTalk.Services.Authentification;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly TwoFactorService _twoFactorService;

    public AuthService(AppDbContext context, TwoFactorService twoFactorService)
    {
        _context = context;
        _twoFactorService = twoFactorService;
    }

    public string HashPassword(string password)
    {
        return PasswordHelper.Hash(password);
    }

    // Register
    public AuthResult Register(string username, string email, string phone, string password, string? type2Fa = null)
    {
        // Validations
        if (username.Length > 255)
            return AuthResult.Failed("Nom d'utilisateur trop long");

        var existingUser = _context.Utilisateurs.FirstOrDefault(u => u.Username == username);
        if (existingUser != null)
            return AuthResult.Failed("Ce nom d'utilisateur est déjà utilisé");

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return AuthResult.Failed("Format d'email incorrect");

        var existingEmail = _context.Utilisateurs.FirstOrDefault(u => u.Email == email);
        if (existingEmail != null)
            return AuthResult.Failed("Cet email est déjà utilisé");

        // Validation mot de passe
        if (password.Contains(' '))
            return AuthResult.Failed("Le mot de passe ne doit pas contenir d'espace");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            return AuthResult.Failed("Le mot de passe doit contenir au moins une majuscule");

        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':\\|,.<>\/?]"))
            return AuthResult.Failed("Le mot de passe doit contenir au moins un caractère spécial");

        if (password.Length < 8)
            return AuthResult.Failed("Le mot de passe doit contenir au moins 8 caractères");

        // Création utilisateur
        string hashedPassword = HashPassword(password);
        var newUser = new Utilisateur(username, email, phone, hashedPassword)
        {
            Type2Fa = type2Fa
        };

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

    // Login
    public AuthResult Login(string username, string password)
    {
        var user = _context.Utilisateurs.FirstOrDefault(u => u.Username == username);

        if (user == null)
            return AuthResult.Failed("Nom d'utilisateur ou mot de passe incorrect");

        if (!user.Actif)
            return AuthResult.Failed("Votre compte a été désactivé");

        bool isMatch = PasswordHelper.Verify(password, user.Password);
        if (!isMatch)
            return AuthResult.Failed("Nom d'utilisateur ou mot de passe incorrect");

        // Si la 2FA est activée
        if (!string.IsNullOrWhiteSpace(user.Type2Fa))
        {
            return new AuthResult
            {
                Success = true,
                User = user,
                Requires2FA = true,
                Token = null
            };
        }

        // Remplace ce token plus tard par un vrai JWT
        return new AuthResult
        {
            Success = true,
            User = user,
            Requires2FA = false,
            Token = "token-temporaire"
        };
    }

    // Vérification du code 2FA
    public AuthResult VerifyTwoFactor(int userId, string code)
    {
        var user = _context.Utilisateurs.FirstOrDefault(u => u.UtilisateurId == userId);

        if (user == null)
            return AuthResult.Failed("Utilisateur introuvable");

        if (string.IsNullOrWhiteSpace(user.Type2Fa))
            return AuthResult.Failed("La double authentification n'est pas activée");

        bool isValid = _twoFactorService.ValidateCode(user.Type2Fa, code);

        if (!isValid)
            return AuthResult.Failed("Code 2FA invalide");

        // Remplace ce token plus tard par un vrai JWT
        return new AuthResult
        {
            Success = true,
            User = user,
            Requires2FA = false,
            Token = "token-temporaire"
        };
    }
}