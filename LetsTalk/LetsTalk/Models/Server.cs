// Server.cs
using LetsTalk.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
namespace LetsTalk.Models;
public class Server
{
    [Key]
    public int ServerId { get; set; }

    [MaxLength(50),Required]
    public string Nom { get; set; }

    // Owner (server.isOwner -> utilisateur.utilisateurId)
    
    public int? OwnerId { get; set; }
    public Utilisateur Owner { get; set; }

    public ICollection<Canaux> Canaux { get; set; }
    public ICollection<Role> Roles { get; set; }
    public ICollection<Membre> Membres { get; set; }
}