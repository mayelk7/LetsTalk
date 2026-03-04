using LetsTalk.Services.Authentification;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.ModelsDto;
using LetsTalk.Models;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;


    [ApiController]
    [Route("api/auth")]
    public class AuthController(AuthService authService) : BaseApiController
    {
        [HttpPost("register")]
        public ApiResponse<UserAuthDto> Register([FromBody] RegisterDto dto)
        {
            var resultReg = authService.Register(
                    dto.Username,
                    dto.Email,
                    dto.Phone,
                    dto.Password
            );
        if (!resultReg.Success)
        {
            return Response<UserAuthDto>(resultReg.ErrorMessage, null);
        }
        var userDtoReg = new UserAuthDto(
            resultReg.User.UtilisateurId ?? 0,
            resultReg.User.Username,
            resultReg.User.Email,
            resultReg.User.Phone,
            resultReg.User.ProfilPicture,
            resultReg.User.Actif,
            resultReg.User.CreatedAt
        );
        return Response("Inscription réussie", userDtoReg);
    }
        [HttpPost("login")]
        public ApiResponse<UserAuthDto> Login([FromBody] LoginDto dto)
        {
            var resultLog = authService.Login(
                dto.Username,
                dto.Password
            );
            if (!resultLog.Success)
            {
                return Response<UserAuthDto>(resultLog.ErrorMessage, null);
            }
        var userDtoLog = new UserAuthDto(
            resultLog.User!.UtilisateurId ?? 0,
            resultLog.User.Username,
            resultLog.User.Email,
            resultLog.User.Phone,
            resultLog.User.ProfilPicture,
            resultLog.User.Actif,
            resultLog.User.CreatedAt
        );
        return Response("Connexion réussie", userDtoLog);
    }
}

