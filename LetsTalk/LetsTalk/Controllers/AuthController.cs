using LetsTalk.Context;
using LetsTalk.Services.Authentification;
using LetsTalk.Services.Email;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService, IEmailService _emailService, AppDbContext _context) : BaseApiController
{
    // Register
    [HttpPost("register")]
    public ApiResponse<UserAuthDto> Register([FromBody] RegisterDto dto)
    {
        var result = authService.Register(
            dto.Username,
            dto.Email,
            dto.Phone,
            dto.Password
        );

        if (!result.Success)
        {
            return Response<UserAuthDto>(result.ErrorMessage, null);
        }

        var userDto = new UserAuthDto(
            result.User!.UtilisateurId ?? 0,
            result.User.Username,
            result.User.Email,
            result.User.Phone,
            result.User.ProfilPicture,
            result.User.Actif,
            result.User.CreatedAt,
            null,
            null,
            null
        );

        return Response("Inscription réussie", userDto);
    }

    // Login
    [HttpPost("login")]
    public async Task<ApiResponse<UserAuthDto>> Login([FromBody] LoginDto dto)
    {
        var result = authService.Login(dto.Username, dto.Password);

        if (!result.Success)
            return Response<UserAuthDto>(result.ErrorMessage, null);

        var userId = result.User!.UtilisateurId ?? 0;

        // Récupère les serveurs owned
        var ownedServerIds = await _context.Servers
            .Where(s => s.OwnerId == userId)
            .Select(s => s.ServerId)
            .ToListAsync();

        // Récupère les rôles par serveur
        var serverRoles = await _context.Membres
            .Where(m => m.UtilisateurId == userId)
            .ToDictionaryAsync(m => m.ServerId, m => m.RoleId);
        
        var serverPermissions = await _context.Membres
           .Where(m => m.UtilisateurId == userId)
           .Include(m => m.Role)
           .ToDictionaryAsync(m => m.ServerId, m => m.Role.Permissions);

        var userDto = new UserAuthDto(
            userId,
            result.User.Username,
            result.User.Email,
            result.User.Phone,
            result.User.ProfilPicture,
            result.User.Actif,
            result.User.CreatedAt,
            ownedServerIds,
            serverRoles,
            serverPermissions
        );

        return Response("Connexion réussie", userDto);
    }

    // Reset Password
    [HttpPost("forgot-password")]

    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        // Demander au service de préparer le reset et récupérer le token
        var token = await authService.PreparePasswordResetAsync(dto.Email);

        if (token == null) return Ok(new { Message = "Si cet email existe, un lien a été envoyé." });

        //  Envoi de l'email
        var scheme = Request.Scheme;
        var host = Request.Host;
        string resetLink = $"{scheme}://{host}/reset-password?token={token}";
        string emailBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #594AE2;'>LetsTalk</h2>
            <p>Bonjour,</p>
            <p>Nous avons reçu une demande de réinitialisation de mot de passe pour votre compte.</p>
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{resetLink}' style='background-color: #594AE2; color: white; padding: 15px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                    Changer mon mot de passe
                </a>
            </div>
            <p style='font-size: 0.8em; color: #666;'>Ce lien expirera prochainement. Si vous n'êtes pas à l'origine de cette demande, vous pouvez ignorer cet e-mail.</p>
            <hr style='border: 0; border-top: 1px solid #eee;' />
            <p style='font-size: 0.7em; color: #999;'>L'équipe LetsTalk</p>
        </div>";

        // Envoie de l'email par le service
        try
        {
            await _emailService.SendEmailAsync(
                dto.Email,
                "Réinitialisation de votre mot de passe",
                emailBody
            );
            return Ok(new { Message = "Email envoyé avec succès" });
        }
        catch (Exception ex)
        {
            // On log l'erreur serveur mais on ne l'affiche pas forcément brute au client
            return StatusCode(500, "Erreur lors de l'envoi de l'email.");
        }
    }

    [HttpPost("reset-password")]
    public async Task<ApiResponse<UserAuthDto>> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        // 3. Appeler la méthode du service pour changer le mot de passe
        var result = await authService.ResetPasswordAsync(dto.Token, dto.NewPassword);

        if (!result.Success)
            return Response<UserAuthDto>(result.ErrorMessage, null);

        var userId = result.User!.UtilisateurId ?? 0;

        // Récupère les serveurs owned
        var ownedServerIds = await _context.Servers
            .Where(s => s.OwnerId == userId)
            .Select(s => s.ServerId)
            .ToListAsync();

        // Récupère les rôles par serveur
        var serverRoles = await _context.Membres
            .Where(m => m.UtilisateurId == userId)
            .ToDictionaryAsync(m => m.ServerId, m => m.RoleId);
        
        var serverPermissions = await _context.Membres
           .Where(m => m.UtilisateurId == userId)
           .Include(m => m.Role)
           .ToDictionaryAsync(m => m.ServerId, m => m.Role.Permissions);

        // Mapping vers le DTO de retour (sans le mot de passe !)
        var userDto = new UserAuthDto(
            result.User.UtilisateurId ?? 0,
            result.User.Username,
            result.User.Email,
            result.User.Phone,
            result.User.ProfilPicture,
            result.User.Actif,
            result.User.CreatedAt,
            ownedServerIds,
            serverRoles,
            serverPermissions
        );

        return Response("Mot de passe modifié avec succès !", userDto);
    }
}