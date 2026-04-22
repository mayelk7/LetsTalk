using System.Net;

namespace LetsTalk.Shared.ModelsDto
{
    public record MemberVoiceDTO
    {
        public string Pseudo { get; set; }
        public IPAddress Adress { get; set; }

        public int Port { get; set; }
    }
}
