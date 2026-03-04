using LetsTalk.Models;
using LetsTalk.Context;
using LetsTalk.Helpers;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Services.Authentification;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
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
        if (password.Contains(" "))
            return AuthResult.Failed("Le mot de passe ne doit pas contenir d'espace");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            return AuthResult.Failed("Le mot de passe doit contenir au moins une majuscule");

        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]"))
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
        {
            return AuthResult.Failed("Nom d'utilisateur ou mot de passe incorrect");
        }

        return AuthResult.Succeeded(user);
    }
}