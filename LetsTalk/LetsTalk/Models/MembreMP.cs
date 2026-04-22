// MembreMP.cs
using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Models;
public class MembreMP
{
    // PK composite (UtilisateurId, ConversationId)
    [Required]
    public int? UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; }

    [Required]
    public int ConversationId { get; set; }          //  FK unique (une seule conversation)
    public ConversationPriver ConversationPriver { get; set; } //  objet unique, pas ICollection
}