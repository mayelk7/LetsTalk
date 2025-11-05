using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // public DbSet<Model> Models { get; set; }
}
