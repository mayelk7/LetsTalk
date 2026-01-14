// Notification.cs
using LetsTalk.Shared;
using System.ComponentModel.DataAnnotations;
using LetsTalk.Shared.Enum;

namespace LetsTalk.Models;
public class Notification
{
    [Key]
    public int NotificationId { get; set; }

    [Required]
    public int? UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; }

    [Required]
    public MessageType MessageType { get; set; }

    [Required]
    public int MessageId { get; set; }

    [Required]
    public bool Lu { get; set; }
}