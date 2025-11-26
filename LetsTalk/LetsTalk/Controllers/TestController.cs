using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await Task.Delay(10);
        return new OkObjectResult("API is working!");
    }
}
