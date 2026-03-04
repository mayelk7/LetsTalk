using Microsoft.EntityFrameworkCore;
using LetsTalk.Models;

namespace LetsTalk.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructeur pour l'injection de dépendances
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet = une "table" dans votre BDD
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Membre> Membres { get; set; }
        public DbSet<MessageCanal> MessagesCanal { get; set; }
        public DbSet<MessagePriver> MessagesPriver { get; set; }
        public DbSet<MembreMP> MembreMPs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<MessageLu> MessageLus { get; set; }
        public DbSet<Server> Servers { get; set; }
    }
}
