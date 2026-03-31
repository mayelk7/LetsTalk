using OtpNet;
using QRCoder;

namespace LetsTalk.Services.Authentication
{
    public class TwoFactorService
    {
        public string GenerateSecret()
        {
            byte[] secretBytes = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(secretBytes);
        }

        public string GenerateOtpAuthUrl(string secret, string email, string issuer)
        {
            string label = Uri.EscapeDataString($"{issuer}:{email}");
            string issuerEncoded = Uri.EscapeDataString(issuer);

            return $"otpauth://totp/{label}?secret={secret}&issuer={issuerEncoded}&digits=6&period=30";
        }

        public string GenerateQrCodeBase64(string otpAuthUrl)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(otpAuthUrl, QRCodeGenerator.ECCLevel.Q);
            var pngQrCode = new PngByteQRCode(qrCodeData);
            byte[] pngBytes = pngQrCode.GetGraphic(20);

            return Convert.ToBase64String(pngBytes);
        }

        public bool ValidateCode(string secret, string code)
        {
            byte[] secretBytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(secretBytes);

            return totp.VerifyTotp(
                code,
                out _,
                new VerificationWindow(previous: 1, future: 1)
            );
        }
    }
}