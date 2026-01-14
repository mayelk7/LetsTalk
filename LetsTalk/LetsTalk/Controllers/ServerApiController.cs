using LetsTalk.Data;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/server")]
public class ServerApiController(BackApiEf db) : BaseApiController
{
    [HttpGet("{id:int}")]
    public FullServerDto? GetServerById(int id)
    {
        var server = db.GetServerById(id);

        if (server == null)
            return null;
            
        var channels = server.Canaux.Select(canal => new ChannelDto(
            canal.CanauxId,
            canal.Nom,
            canal.Messages.Select(msg => new MessageDto(
                msg.MessageId,
                new UserDto(
                    msg.Utilisateur.UtilisateurId,
                    msg.Utilisateur.Username
                ),
                msg.Contenu,
                msg.DateEnvoi
            )).ToList(),
            canal.Type
        )).ToList();

        var users = server.Membres.Select(user => new UserDto(
            user.UtilisateurId,
            user.Utilisateur.Username
        )).ToList();
            
        return new FullServerDto(
            server.ServerId,
            server.Nom,
            channels,
            users
        );
    }
}