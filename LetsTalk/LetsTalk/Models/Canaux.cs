// Canaux.cs
using LetsTalk.Shared;
using System.ComponentModel.DataAnnotations;
namespace LetsTalk.Models;

public class Canaux
{
    [Key,Required]
    public int CanauxId { get; set; }
    [Required]
    public int ServerId { get; set; }
    public Server Server { get; set; }

    [Required, MaxLength(50)]
    public string Nom { get; set; }
    [Required]
    public ChannelType Type { get; set; }

    public ICollection<MessageCanal> Messages { get; set; }
}