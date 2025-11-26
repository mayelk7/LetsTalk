using Microsoft.EntityFrameworkCore;
using LetsTalk.Models;

namespace LetsTalk.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // public DbSet<Model> Models { get; set; }
    public DbSet<Utilisateur> Utilisateurs { get; set; }
}
