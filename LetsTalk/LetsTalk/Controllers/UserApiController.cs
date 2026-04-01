using LetsTalk.Context;
using LetsTalk.Data;
using LetsTalk.Models;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;



namespace LetsTalk.Controllers;

[ApiController]
[Route("api/user")]
public class UserApiController(BackApiEf _db, AppDbContext context, AuthenticationStateProvider authStateProvider) : BaseApiController
{
    private readonly AppDbContext _context = context;
    private readonly AuthenticationStateProvider _authStateProvider;


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
        return user == null
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
    public record LeaveServerRequest(string UserId);

    [HttpPut("servers/{id}/leave")]
    public async Task<ActionResult<ApiResponse<bool>>> LeaveServer(int id, [FromBody] LeaveServerRequest request)
    {
        System.Console.WriteLine($"Request reçue: id={id}, userId={request?.UserId}");
        if (string.IsNullOrEmpty(request.UserId))
            return Unauthorized(new ApiResponse<bool>(false, "Utilisateur non identifié.", false));

        try
        {
            var serverEntity = await _context.Servers
                .Include(s => s.Membres)
                .FirstOrDefaultAsync(s => s.ServerId == id);

            if (serverEntity == null)
                return NotFound(new ApiResponse<bool>(false, "Serveur introuvable.", false));

            var userToRemove = serverEntity.Membres.FirstOrDefault(u => u.UtilisateurId.ToString() == request.UserId);

            if (userToRemove != null)
            {
                serverEntity.Membres.Remove(userToRemove);
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse<bool>(true, "Vous avez quitté le serveur.", true));
            }

            return BadRequest(new ApiResponse<bool>(false, "Vous n'êtes pas membre de ce serveur.", false));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>(false, ex.Message, false));
        }
    }

    [HttpGet("{userid:int}/servers/")]
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
