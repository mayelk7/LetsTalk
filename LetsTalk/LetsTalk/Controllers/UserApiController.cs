using LetsTalk.Data;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/user")]
public class UserApiController(BackApiEf _db) : BaseApiController
{
    [HttpGet("")]
    public ApiResponse<List<UserDto>> GetAllUser()
    {
        return Response("Liste des utilisateurs récupérée avec succès", _db.GetAllUsers().Select(user => new UserDto(
            user.UtilisateurId,
            user.Username,
            user.Email,
            user.Phone,
            user.ProfilPicture,
            user.CreatedAt
        )).ToList());
    }
    
    [HttpGet("{id:int}")]
    public UserDto? GetUser(int id)
    {
        var user = _db.GetUserById(id);
        return  user == null 
            ? null 
            : new UserDto(
                user.UtilisateurId,
                user.Username,
                user.Email,
                user.Phone,
                user.ProfilPicture,
                user.CreatedAt
            );
    }
    
    /// <summary>
    ///     Retrieve all servers linked to a user
    /// </summary>
    /// <param name="userid"></param>
    /// <returns>
    ///     List of UserServerDto representing the servers associated with the specified user ID
    /// </returns>
    [HttpGet("{userid:int}/servers")]
    public ApiResponse<List<UserServerDto>> GetUserServers(int userid)
    {
        var servers = _db.GetUserServers(userid);

        return Response("Liste des serveurs de l'utilisateur récupérée avec succès",
            servers.Select(
                server => new UserServerDto(userid, server.ServerId, server.Nom)
                ).ToList()
            );
    }
}
