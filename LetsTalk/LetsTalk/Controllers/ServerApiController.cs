using LetsTalk.Context;
using LetsTalk.Data;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/server")]
public class ServerApiController(AppDbContext appDbContext) : BaseApiController
{
    [HttpGet("{id:int}")]
    public ApiResponse<FullServerDto> GetServerById(int id)
    {
        var server = appDbContext.Servers
            .Include(s => s.Canaux)
                .ThenInclude(c => c.Messages)
                    .ThenInclude(messageCanal => messageCanal.Utilisateur)
            .Include(s => s.Membres)
                .ThenInclude(m => m.Utilisateur)
            .FirstOrDefault(s => s.ServerId == id);

        if (server == null)
            return this.Response<FullServerDto>("Serveur non trouvé.", null);
            
        var channels = server.Canaux.Select(canal => new ChannelDto(
            canal.CanauxId,
            canal.Nom,
            canal.Messages.Select(msg => new MessageDto(
                msg.MessageId,
                new UserDto(
                    msg.Utilisateur.UtilisateurId,
                    msg.Utilisateur.Username,
                    msg.Utilisateur.Email,
                    msg.Utilisateur.Phone,
                    msg.Utilisateur.ProfilPicture,
                    msg.Utilisateur.CreatedAt
                ),
                msg.Contenu,
                msg.DateEnvoi
            )).ToList(),
            canal.Type
        )).ToList();

        var users = server.Membres.Select(user => new UserDto(
            user.UtilisateurId,
            user.Utilisateur.Username,
            user.Utilisateur.Email,
            user.Utilisateur.Phone,
            user.Utilisateur.ProfilPicture,
            user.Utilisateur.CreatedAt
        )).ToList();
            
        return this.Response<FullServerDto>("Server retrouvé.", new FullServerDto(
            server.ServerId,
            server.Nom,
            channels,
            users
        ));
    }
}
