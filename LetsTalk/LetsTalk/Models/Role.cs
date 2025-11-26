// Role.cs
using System;
using System.ComponentModel.DataAnnotations;
namespace LetsTalk.Models;
public class Role
{
    [Key]
    public int RoleId { get; set; }

    [Required,MaxLength(50)]
    public string Nom { get; set; }

    [Required]
    public int Level { get; set; }
    [Required]
    public int ServerId { get; set; }
    public Server Server { get; set; }

    public ICollection<Membre> Membres { get; set; }
}