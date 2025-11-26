using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Models;

[PrimaryKey(nameof(Id))]
public class Utilisateur
{
    public int Id { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    [Required]
    public string Username { get; set; }
    
    [Column(TypeName = "varchar(300)")]
    [Required]
    public string Email { get; set; }
    
    [Column(TypeName = "varchar(10)")]
    [Phone]
    public string Phone { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    public string ProfilePicture { get; set; }
    
    [Required]
    public bool Actif { get; set; }
    
    public string Type2FA { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
}
