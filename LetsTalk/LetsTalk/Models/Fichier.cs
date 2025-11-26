// Fichier.cs
using LetsTalk.Shared;
using System.ComponentModel.DataAnnotations;
namespace Models;
public class Fichier
{
    [Key]
    public int FichierId { get; set; }

    [Required, MaxLength(50)]
    public string Nom { get; set; }

    [Required]
    public string Url { get; set; }

    [Required, MaxLength(50)]
    public string Type { get; set; }

    // messageType + messageId (no FK to two possible tables)
    [Required]
    public MessageType MessageType { get; set; }

    [Required]
    public int MessageId { get; set; }
}