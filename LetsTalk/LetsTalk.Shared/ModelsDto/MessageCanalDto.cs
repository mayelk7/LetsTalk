namespace LetsTalk.Shared.ModelsDto;

public class MessageCanalDto
{
    public int MessageId { get; set; }
    public string Contenu { get; set; }
    public DateTime DateEnvoi { get; set; }

    public int UtilisateurId { get; set; }
    public string Username { get; set; }

    public int CanalId { get; set; }
    public string NomCanal { get; set; }
}
