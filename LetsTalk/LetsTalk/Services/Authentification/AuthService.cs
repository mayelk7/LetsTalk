using LetsTalk.Context;
using LetsTalk.Helpers;
using LetsTalk.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static QRCoder.PayloadGenerator;

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
    // Passwd reset
    public async Task<string?> PreparePasswordResetAsync(string email)
    {
        // 1. Trouver l'utilisateur par son email
        var user = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null) return null;

        // 2. Générer un token unique
        string token =Guid.NewGuid().ToString();

        // 3. Enregistrer le token et l'expiration
        user.PasswordResetToken = token;
        user.ResetTokenExpires = DateTime.Now.AddMinutes(15);

        await _context.SaveChangesAsync();

        return token;
    }
    // Reset password
    public async Task<AuthResult> ResetPasswordAsync(string token, string newPassword)
    {
        // 1. Trouver l'utilisateur qui possède CE token précis
        var user = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.PasswordResetToken == token);

        // 2. Vérifications de sécurité
        if (user == null || user.ResetTokenExpires < DateTime.Now)
        {
            return AuthResult.Failed("Le lien est invalide ou a expiré.");
        }

        try
        {
            // 3. Hacher le nouveau mot de passe et nettoyer les champs de reset
            string hashed = HashPassword(newPassword);
            user.SetNewPassword(hashed);

            // Save changes
            await _context.SaveChangesAsync();
            return AuthResult.Succeeded(user);
        }
        catch(ArgumentException ex) 
        {
            return AuthResult.Failed(ex.Message);
        }
        catch (Exception)
        {
            return AuthResult.Failed("Une erreur est survenue lors de la modification.");
        }    
    }
}
