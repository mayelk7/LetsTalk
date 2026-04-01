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
            result.User.UtilisateurId,
            result.User.Username,
            result.User.Email,
            result.User.Phone,
            result.User.ProfilPicture,
            result.User.Actif,
            result.User.CreatedAt

        );

        return Response("Inscription réussie", userDto);
    }

    [HttpPost("login")]
    public ApiResponse<UserAuthDto> Login([FromBody] LoginDto dto)
    {
        var result = authService.Login(dto.Username, dto.Password);

        if (!result.Success)
            return Response<UserAuthDto>(result.ErrorMessage, null);

        if (result.User is null)
            return Response<UserAuthDto>("Utilisateur introuvable", null);

        if (result.Requires2FA)
            return Response<UserAuthDto>("2FA requis", null, requires2FA: true); // ✅ pas de token ici

        var userDto = new UserAuthDto(
            result.User.UtilisateurId,
            result.User.Username,
            result.User.Email,
            result.User.Phone,
            result.User.ProfilPicture,
            result.User.Actif,
            result.User.CreatedAt
        );

        return Response("Connexion réussie", userDto, token: result.Token); // ✅ token inclus
    }

    [HttpPost("verify-2fa")]
    public ApiResponse<UserAuthDto> Verify2FA([FromBody] VerifyTwoFactorDto dto)
    {
        var result = authService.VerifyTwoFactor(dto.UserId, dto.Code);

        if (!result.Success)
            return Response<UserAuthDto>(result.ErrorMessage, null);

        if (result.User is null)
            return Response<UserAuthDto>("Utilisateur introuvable", null);

        var userDto = new UserAuthDto(
            result.User.UtilisateurId,
            result.User.Username,
            result.User.Email,
            result.User.Phone,
            result.User.ProfilPicture,
            result.User.Actif,
            result.User.CreatedAt
        );

        return Response("Vérification réussie", userDto, token: result.Token); // ✅ token inclus
    }
}