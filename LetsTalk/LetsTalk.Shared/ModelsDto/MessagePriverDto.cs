namespace LetsTalk.Shared.ModelsDto
{
    public class MessagePriverDto
    {
        public int MessageId { get; set; }
        public string Contenu { get; set; }
        public DateTime DateEnvoi { get; set; }

        public int UtilisateurId { get; set; }
        public string Username { get; set; }

        public int ConversationPriverId { get; set; }

        public List<MembreMPDto> MembreMPs { get; set; }
    }

}
