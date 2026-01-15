using LetsTalk.Data;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/canaux")]
public class CanauxApiController(BackApiEf _db) : BaseApiController
{
    [HttpGet("")]
    public List<MessageCanalDto> GetAllMessageCanal()
    {
        return _db.GetAllMessagesCanal();
    }
}
