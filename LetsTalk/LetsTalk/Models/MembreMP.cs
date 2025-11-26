// MembreMP.cs
using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Models;
public class MembreMP
{
    // PK composite (UtilisateurId, ConversationId)
    [Required]
    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; }

    [Required]
    public int ConversationId { get; set; }
    public ConversationPriver ConversationPriver { get; set; }
}