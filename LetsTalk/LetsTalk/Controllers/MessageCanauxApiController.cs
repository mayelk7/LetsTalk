using LetsTalk.Context;
using LetsTalk.Models;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/messagecanaux")]
public class MessageCanauxApiController(AppDbContext appDbContext) : BaseApiController
{
    [HttpPost("")]
    public ApiResponse<object> Create([FromBody] MessageDto messageDto)
    {
        var message = new MessageCanal()
        {
            Contenu = messageDto.Content,
            DateEnvoi = messageDto.Timestamp,
            
        };
        
        // appDbContext.MessagesCanal.Add()
        
        return new ApiResponse<object>(
            true,
            "Message created successfully",
            null
        );
    }
}
