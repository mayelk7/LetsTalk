using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Client.Context;

public class DbContext(DbContextOptions<DbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    // public DbSet<Model> Models { get; set; }
}