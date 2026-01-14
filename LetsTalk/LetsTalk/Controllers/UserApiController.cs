using LetsTalk.Data;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/user")]
public class UserApiController(BackApiEf _db) : BaseApiController
{
    // [HttpGet("GetAllUser")]
    // public List<Utilisateur> GetAllUser()
    // {
    //     return _db.GetAllUsers();
    // }
    
    /// <summary>
    ///     Retrieve all servers linked to a user
    /// </summary>
    /// <param name="userid"></param>
    /// <returns>
    ///     List of UserServerDto representing the servers associated with the specified user ID
    /// </returns>
    [HttpGet("{userid:int}/servers")]
    public List<UserServerDto> GetUserServers(int userid)
    {
        var servers = _db.GetUserServers(userid);

        return servers.Select(server => new UserServerDto(userid, server.ServerId, server.Nom)).ToList();
    }
}