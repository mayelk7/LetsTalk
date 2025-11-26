// MessageLu.cs
using LetsTalk.Shared;
using System.ComponentModel.DataAnnotations;
namespace LetsTalk.Models;

public class MessageLu
{
    // composite PK (utilisateurId, messageType, messageId)
    [Required]
    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; }
    [Required]
    public MessageType MessageType { get; set; }
    [Required]
    public int MessageId { get; set; }
    [Required,DeniedValues(false)]
    public bool Lu { get; set; }
}