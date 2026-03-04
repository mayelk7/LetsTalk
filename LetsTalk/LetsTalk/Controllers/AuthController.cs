using LetsTalk.Services.Authentification;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : BaseApiController
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
            result.User.CreatedAt
        );

        return Response("Inscription réussie", userDto);
    }

    // Login
    [HttpPost("login")]
    public ApiResponse<UserAuthDto> Login([FromBody] LoginDto dto)
    {
        var result = authService.Login(dto.Username, dto.Password);

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
            result.User.CreatedAt
        );

        return Response("Connexion réussie", userDto);
    }
}