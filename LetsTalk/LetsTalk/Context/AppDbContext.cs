using LetsTalk.Models;
using Microsoft.EntityFrameworkCore;
using LetsTalk.Models;

namespace LetsTalk.Context;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<Server> Servers { get; set; }
    public DbSet<Canaux> Canaux { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Membre> Membres { get; set; }
    public DbSet<MessageCanal> MessagesCanal { get; set; }
    public DbSet<ConversationPriver> ConversationPrivers { get; set; }
    public DbSet<MembreMP> MembreMPs { get; set; }
    public DbSet<MessagePriver> MessagesPriver { get; set; }
    public DbSet<Fichier> Fichiers { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<MessageLu> MessageLus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Table names (optional, to match your names exactly)
        modelBuilder.Entity<Utilisateur>().ToTable("utilisateur");
        modelBuilder.Entity<Server>().ToTable("server");
        modelBuilder.Entity<Canaux>().ToTable("canaux");
        modelBuilder.Entity<Role>().ToTable("role");
        modelBuilder.Entity<Membre>().ToTable("Membre");
        modelBuilder.Entity<MessageCanal>().ToTable("messageCanal");
        modelBuilder.Entity<ConversationPriver>().ToTable("ConversationPriver");
        modelBuilder.Entity<MembreMP>().ToTable("membreMP");
        modelBuilder.Entity<MessagePriver>().ToTable("messagePriver");
        modelBuilder.Entity<Fichier>().ToTable("fichier");
        modelBuilder.Entity<Notification>().ToTable("notification");
        modelBuilder.Entity<MessageLu>().ToTable("messageLu");

        // ENUMS -> store as strings (to match SQL ENUM text)
        modelBuilder
            .Entity<Canaux>()
            .Property(c => c.Type)
            .HasConversion<string>();

        modelBuilder
            .Entity<Fichier>()
            .Property(f => f.MessageType)
            .HasConversion<string>();

        modelBuilder
            .Entity<Notification>()
            .Property(n => n.MessageType)
            .HasConversion<string>();

        modelBuilder
            .Entity<MessageLu>()
            .Property(m => m.MessageType)
            .HasConversion<string>();

        // Composite PK for Membre (UtilisateurId, ServerId, RoleId)
        modelBuilder.Entity<Membre>()
            .HasKey(m => new { m.UtilisateurId, m.ServerId, m.RoleId });

        // Composite PK for MembreMP
        modelBuilder.Entity<MembreMP>()
            .HasKey(mm => new { mm.UtilisateurId, mm.ConversationId });

        // Composite PK for MessageLu
        modelBuilder.Entity<MessageLu>()
            .HasKey(ml => new { ml.UtilisateurId, ml.MessageType, ml.MessageId });

        // Relations
        modelBuilder.Entity<Server>()
            .HasMany(s => s.Canaux)
            .WithOne(c => c.Server)
            .HasForeignKey(c => c.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Server>()
            .HasMany(s => s.Roles)
            .WithOne(r => r.Server)
            .HasForeignKey(r => r.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Server>()
            .HasMany(s => s.Membres)
            .WithOne(m => m.Server)
            .HasForeignKey(m => m.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Server owner
        modelBuilder.Entity<Server>()
            .HasOne(s => s.Owner)
            .WithMany(u => u.OwnedServers)
            .HasForeignKey(s => s.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Role>()
            .HasMany(r => r.Membres)
            .WithOne(m => m.Role)
            .HasForeignKey(m => m.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Utilisateur>()
            .HasMany(u => u.Membres)
            .WithOne(m => m.Utilisateur)
            .HasForeignKey(m => m.UtilisateurId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Canaux>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Canal)
            .HasForeignKey(m => m.CanalId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Utilisateur>()
            .HasMany(u => u.MessagesCanal)
            .WithOne(m => m.Utilisateur)
            .HasForeignKey(m => m.UtilisateurId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ConversationPriver>()
            .HasMany(c => c.MembreMPs)
            .WithOne(mp => mp.ConversationPriver)
            .HasForeignKey(mp => mp.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ConversationPriver>()
            .HasMany(c => c.MessagesPriver)
            .WithOne(m => m.ConversationPriver)
            .HasForeignKey(m => m.ConversationPriverId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Utilisateur>()
            .HasMany(u => u.MessagesPriver)
            .WithOne(m => m.Utilisateur)
            .HasForeignKey(m => m.UtilisateurId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notification: link to user
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Utilisateur)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UtilisateurId)
            .OnDelete(DeleteBehavior.Cascade);

        // MessageLu -> link to user
        modelBuilder.Entity<MessageLu>()
            .HasOne(ml => ml.Utilisateur)
            .WithMany(u => u.MessageLus)
            .HasForeignKey(ml => ml.UtilisateurId)
            .OnDelete(DeleteBehavior.Cascade);

        // Fichier, Notification, MessageLu: no FK to message tables (messageId + messageType used in app logic)

        // Indexes (optional but useful)
        modelBuilder.Entity<MessageCanal>()
            .HasIndex(m => new { m.CanalId, m.DateEnvoi });

        modelBuilder.Entity<MessagePriver>()
            .HasIndex(m => new { m.ConversationPriverId, m.DateEnvoi });
    }
}
