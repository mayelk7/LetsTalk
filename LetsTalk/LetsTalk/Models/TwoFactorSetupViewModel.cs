namespace LetsTalk.Models
{
    public class TwoFactorSetupViewModel
    {
        public string Secret { get; set; } = "";
        public string QrCodeBase64 { get; set; } = "";
        public string OtpAuthUrl { get; set; } = "";
    }
}