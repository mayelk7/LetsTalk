using System.Text.Json;
using LetsTalk.Context;
using LetsTalk.Models;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.Enum;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/messagecanaux")]
public class MessageCanauxApiController(AppDbContext appDbContext) : BaseApiController
{
    [HttpPost("")]
    public async Task<ApiResponse<MessageCanalDto?>> Create([FromBody] MessageDto messageDto)
    {
        var message = new MessageCanal()
        {
            Contenu = messageDto.Content,
            DateEnvoi = messageDto.Timestamp,
            UtilisateurId = messageDto.Sender.Id,
            CanalId = messageDto.CanalId
        };
        
        Console.WriteLine(JsonSerializer.Serialize(message));

        var messageEntity = await appDbContext.MessagesCanal.AddAsync(message);
        
        var fichier = messageDto.Fichier != null
            ? new Fichier
            {
                Nom = messageDto.Fichier?.Nom ?? "Inconnu",
                Url = messageDto.Fichier?.Url ?? "",
                Type = "0",
                MessageType = MessageType.Canal,
                MessageId = messageEntity.Entity.MessageId
            }
            : null;
        
        
        if (fichier != null)
        {
            await appDbContext.Fichiers.AddAsync(fichier);
        }
        
        await appDbContext.SaveChangesAsync();

        // var messageCanalDto = appDbContext.MessagesCanal
        //     .AsNoTracking()
        //     .Where(m => m.Contenu == messageDto.Content
        //                 && m.DateEnvoi == messageDto.Timestamp)
        //     .Select(m => new MessageCanalDto(
        //         m.MessageId,
        //         m.Contenu,
        //         m.DateEnvoi,
        //         m.UtilisateurId,
        //         m.Utilisateur.Username,
        //         m.CanalId,
        //         m.Canal.Nom
        //     )).FirstOrDefault();
        
        return new ApiResponse<MessageCanalDto?>(
            true,
            "Message created successfully",
            null
        );
    }
    
    [HttpGet("canal/{id:int}")]
    public ApiResponse<List<MessageCanalDto>> GetMessagesByCanalId(int id)
    {
        var messages = appDbContext.MessagesCanal
            .AsNoTracking()
            .Where(m => m.CanalId == id)
            .Select(m => new MessageCanalDto(
                m.MessageId,
                m.Contenu,
                m.DateEnvoi,
                m.UtilisateurId,
                m.Utilisateur.Username,
                m.CanalId,
                m.Canal.Nom,
                m.Fichier != null ? m.Fichier.Url : null
            )).ToList();

        return new ApiResponse<List<MessageCanalDto>>(
            true,
            "Messages retrieved successfully",
            messages
        );
    }
}
