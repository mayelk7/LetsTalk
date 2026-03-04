using LetsTalk.Services.Livekit;
using LetsTalk.Data;
using Microsoft.AspNetCore.Mvc;
using LetsTalk.Shared.Api;
using LetsTalk.Shared.ModelsDto;

namespace LetsTalk.Controllers
{

    [ApiController]
    [Route("api/voice")]
    public class LivekitApiController : BaseApiController
    {
        private readonly LivekitService _livekit;
        private readonly BackApiEf _backapi;

        public LivekitApiController(LivekitService livekit, BackApiEf backapi)
        {
            _livekit = livekit;
            _backapi = backapi;
        }

        // GET api/voice/{roomName}/members
        [HttpGet("{roomName}/members")]
        public async Task<ApiResponse<List<ParticipantLiveKit>>> GetMembers(string roomName)
        {
            var members = await _backapi.GetMembersWithDbInfo(roomName);
            return BaseApiController.Response<List<ParticipantLiveKit>>("Members retrieved successfully", members);
        }

        // GET api/voice/rooms
        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _livekit.ListerSalonsActifs();
            return Ok(rooms);
        }
    }

    
}
