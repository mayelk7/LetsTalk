using LetsTalk.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly TwoFactorService _twoFactorService;

        public AccountController(TwoFactorService twoFactorService)
        {
            _twoFactorService = twoFactorService;
        }

        [HttpGet("setup-2fa")]
        public IActionResult Setup2Fa([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email requis.");

            string issuer = "LetsTalk";
            string secret = _twoFactorService.GenerateSecret();
            string otpAuthUrl = _twoFactorService.GenerateOtpAuthUrl(secret, email, issuer);
            string qrCodeBase64 = _twoFactorService.GenerateQrCodeBase64(otpAuthUrl);

            return Ok(new
            {
                secret,
                otpAuthUrl,
                qrCodeBase64
            });
        }

        [HttpPost("verify-2fa")]
        public IActionResult Verify2Fa([FromBody] Verify2FaRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Secret) || string.IsNullOrWhiteSpace(request.Code))
                return BadRequest("Secret et code requis.");

            bool isValid = _twoFactorService.ValidateCode(request.Secret, request.Code);

            if (!isValid)
                return BadRequest(new { message = "Code invalide" });

            return Ok(new { message = "Code valide" });
        }
    }

    public class Verify2FaRequest
    {
        public string Secret { get; set; } = "";
        public string Code { get; set; } = "";
    }
}
