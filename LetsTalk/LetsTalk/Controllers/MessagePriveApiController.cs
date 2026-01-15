using LetsTalk.Data;
using LetsTalk.Shared.ModelsDto;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/messageprive")]
public class MessagePriveApiController(BackApiEf _db) : BaseApiController
{
    [HttpGet("")]
    public List<MessagePriverDto> GetAllMessagePriver()
    {
        return _db.GetAllMessagesPriver();
    }
}
