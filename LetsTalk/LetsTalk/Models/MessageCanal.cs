// MessageCanal.cs

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Models;
public class MessageCanal
{
    [Key]
    public int MessageId { get; set; }
    [Required,MaxLength(2000)]
    public string Contenu { get; set; }

    [Required]
    public DateTime DateEnvoi { get; set; }

    [Required,DefaultValue(false)]
    public bool Epingle { get; set; }

    [Required]
    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; }

    [Required]
    public int CanalId { get; set; }
    public Canaux Canal { get; set; }
}