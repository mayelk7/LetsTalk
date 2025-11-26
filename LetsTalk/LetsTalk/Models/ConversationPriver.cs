// ConversationPriver.cs
using System.ComponentModel.DataAnnotations;
namespace LetsTalk.Models;
public class ConversationPriver
{
    [Key,Required]
    public int ConversationPriverId { get; set; }
    [Required]
    public DateTime? CreatedAt { get; set; }

    public ICollection<MembreMP> MembreMPs { get; set; }
    public ICollection<MessagePriver> MessagesPriver { get; set; }
}