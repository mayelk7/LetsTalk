using LetsTalk.Context;
using LetsTalk.Data;
using LetsTalk.Models;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.Enum;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/canaux")]
public class CanauxApiController(BackApiEf _db, AppDbContext appDbContext) : BaseApiController
{
    [HttpGet("")]
    public List<MessageCanalDto> GetAllMessageCanal()
    {
        return _db.GetAllMessagesCanal();
    }

    [HttpPost("newcanal")]
    public async Task<ApiResponse<bool>> CreateCanal([FromBody] CreatecanalDto dto)
    {
    
            // 2. Création du nouveau canal à partir du DTO
            var nouveauCanal = new Canaux
            {
                Nom = dto.Nom,
                ServerId = dto.serverid,
                Type = dto.Type
            };

            appDbContext.Canaux.Add(nouveauCanal);

            // 3. Sauvegarde asynchrone
            var result = await appDbContext.SaveChangesAsync();
            if (result == 0)
                return Response<bool>("Erreur lors de la création du serveur.", false);
            else
                return Response<bool>("Canal créé avec succès.", true);
         
    
    }
    
}