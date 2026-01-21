using System.Text.Json;
using LetsTalk.Context;
using LetsTalk.Models;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/messagecanaux")]
public class MessageCanauxApiController(AppDbContext appDbContext) : BaseApiController
{
    [HttpPost("")]
    public ApiResponse<MessageCanalDto?> Create([FromBody] MessageDto messageDto)
    {
        var message = new MessageCanal()
        {
            Contenu = messageDto.Content,
            DateEnvoi = messageDto.Timestamp,
            UtilisateurId = messageDto.Sender.Id,
            CanalId = messageDto.Id ?? 0
        };
        
        Console.WriteLine(JsonSerializer.Serialize(message));

        appDbContext.MessagesCanal.AddAsync(message);
        appDbContext.SaveChangesAsync();

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
                m.Canal.Nom
            )).ToList();

        return new ApiResponse<List<MessageCanalDto>>(
            true,
            "Messages retrieved successfully",
            messages
        );
    }
}
