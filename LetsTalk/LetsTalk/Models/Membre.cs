// Membre.cs
namespace LetsTalk.Models;
public class Membre
{
    // Composite PK (UtilisateurId, ServerId, RoleId)
    public int? UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; }

    public int ServerId { get; set; }
    public Server Server { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; }

}