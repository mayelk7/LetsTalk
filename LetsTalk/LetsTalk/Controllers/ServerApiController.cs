using LetsTalk.Context;
using LetsTalk.Data;
using LetsTalk.Models;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.Enum;
using LetsTalk.Shared.ModelsDto;
using Livekit.Server.Sdk.Dotnet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

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
            return Response<FullServerDto>("Serveur non trouvé.", null);
            
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
                msg.DateEnvoi,
                msg.CanalId,
                msg.Epingle
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
            
        return Response<FullServerDto>("Server retrouvé.", new FullServerDto(
            server.ServerId,
            server.Nom,
            channels,
            users
        ));
    }

    [HttpPost]
    public ApiResponse<bool> SetNewServer([FromBody] CreateServerDto dto)
    {
        var server = new Server { Nom = dto.Nom, OwnerId = dto.OwnerId };
        appDbContext.Servers.Add(server);

        if (appDbContext.SaveChanges() == 0)
            return Response<bool>("Erreur lors de la création du serveur.", false);

        var canal = new Canaux
        {
            Nom = "Général",
            ServerId = server.ServerId,
            Type = ChannelType.Text
        };
        appDbContext.Canaux.Add(canal);

        if (dto.Membres != null && dto.Membres.Any())
        {
            foreach (var userId in dto.Membres)
            {
                appDbContext.Membres.Add(new Membre
                {
                    UtilisateurId = userId,
                    ServerId = server.ServerId,
                    RoleId = 1
                });
            }
        }

        appDbContext.SaveChanges();
        return Response<bool>("Serveur créé avec succès.", true);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateServer(int id, [FromBody] FullServerDto serverDto)
    {
        // 1. Vérification de sécurité
        if (serverDto == null || id != serverDto.Id)
        {
            return BadRequest(new ApiResponse<bool> (false,"Données invalides.",false ));
        }

        try
        {
            // 2. On cherche le serveur en base de données
            var serverEntity = await appDbContext.Servers.FirstOrDefaultAsync(s => s.ServerId == id);

            if (serverEntity == null)
            {
                return NotFound(new ApiResponse<bool>(false, "Serveur introuvable.", false)); ;
            }

            // 3. Mise à jour des propriétés
            // On change le nom par celui reçu du Blazor
            serverEntity.Nom = serverDto.Name;

            // 4. On sauvegarde les changements
            await appDbContext.SaveChangesAsync();

            return Ok(new ApiResponse<bool> (true,"Serveur mis à jour !",true ));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool> (false,ex.Message,false));
        }
    }

};

