using LetsTalk.Services.Authentification;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : BaseApiController
{
    [HttpPost("register")]
    public ApiResponse<UserAuthDto> Register([FromBody] RegisterDto dto)
    {
        var result = authService.Register(
            dto.Username,
            dto.Email,
            dto.Phone,
            dto.Password
        );

        if (!result.Success || result.User is null)
        {
            return Response<UserAuthDto>(result.ErrorMessage, null);
        }

        var userDto = new UserAuthDto(
            result.User.UtilisateurId ?? 0,
            result.User.Username,
            result.User.Email,
            result.User.Phone,
            result.User.ProfilPicture,
            result.User.CreatedAt,
            result.User.Actif,

        )

        return Response("Inscription réussie", userDto);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var result = authService.Login(dto.Username, dto.Password);

        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = result.ErrorMessage
            });
        }

        if (result.User is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Utilisateur introuvable"
            });
        }

        if (result.Requires2FA)
        {
            return Ok(new
            {
                success = true,
                requires2FA = true,
                userId = result.User.UtilisateurId
            });
        }

        var userDto = new UserAuthDto(
            result.User.UtilisateurId ?? 0,
            result.User.Username,
            result.User.Email,
            result.User.Phone,
            result.User.ProfilPicture,
            result.User.CreatedAt,
            result.User.Actif,

        );

        return Ok(new
        {
            success = true,
            requires2FA = false,
            token = result.Token,
            user = userDto
        });
    }

    [HttpPost("verify-2fa")]
    public IActionResult Verify2FA([FromBody] VerifyTwoFactorDto dto)
    {
        var result = authService.VerifyTwoFactor(dto.UserId, dto.Code);

        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = result.ErrorMessage
            });
        }

        if (result.User is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Utilisateur introuvable"
            });
        }

        var userDto = new UserAuthDto(
            result.User.UtilisateurId ?? 0,
            result.User.Username,
            result.User.Email,
            result.User.Phone,
            result.User.ProfilPicture,
            result.User.CreatedAt,
            result.User.Actif,

        );

        return Ok(new
        {
            success = true,
            token = result.Token,
            user = userDto
        });
    }
}