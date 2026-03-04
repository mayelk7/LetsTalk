using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace LetsTalk.Models;

[Index(nameof(Username), [nameof(Email)], IsUnique = true)]
[PrimaryKey("UtilisateurId")]
public class Utilisateur
{
    [Key]
    public int? UtilisateurId { get; set; }

    [Required, MaxLength(255)]
    public string Username { get; set; }

    [Required, MaxLength(255)]
    public string Email { get; set; }

    [Required,MaxLength(10)]
    public string Phone { get; set; }

    [Required, MaxLength(255)]
    public string Password { get; set; }
    
    public string? ProfilPicture { get; set; }

    [Required,DefaultValue(true)]
    public bool Actif { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [MaxLength(50)]
    public string? Type2Fa { get; set; }

    // Navigations

    public ICollection<Membre> Membres { get; set; }
    public ICollection<MessageCanal> MessagesCanal { get; set; }
    public ICollection<MessagePriver> MessagesPriver { get; set; }
    public ICollection<MembreMP> MembreMPs { get; set; }
    public ICollection<Notification> Notifications { get; set; }
    public ICollection<MessageLu> MessageLus { get; set; }


    // Owned servers (if user is owner referenced by Server.OwnerId)
    public ICollection<Server> OwnedServers { get; set; }

    //Constructors

    public Utilisateur()
    {
        CreatedAt = DateTime.UtcNow;
        Membres = new List<Membre>();
        MessagesCanal = new List<MessageCanal>();
        MessagesPriver = new List<MessagePriver>();
        MembreMPs = new List<MembreMP>();
        Notifications = new List<Notification>();
        MessageLus = new List<MessageLu>();
    }
    public Utilisateur(string username, string email, string phone, string hashedPassword) : this()
    {
        Username = username;
        Email = email;
        Phone = phone;
        Password = hashedPassword;
        Actif = true;
    }

}
