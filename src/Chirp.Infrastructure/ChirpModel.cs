using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpContext(DbContextOptions<ChirpContext> options) : DbContext(options)
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Reaction> Reactions { get; set; }

    public void InitializeDatabase(bool seedDatabase)
    {
        Database.EnsureCreated();
        if (seedDatabase) DbInitializer.SeedDatabase(this);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cheep>().Property(c => c.Message).HasMaxLength(160);

        modelBuilder.Entity<Author>().Property(a => a.Name).HasMaxLength(32);
        modelBuilder.Entity<Author>().HasIndex(a => a.Name).IsUnique();
        modelBuilder.Entity<Author>().Property(a => a.Email).HasMaxLength(300);
        modelBuilder.Entity<Author>().Property(a => a.DisplayName).HasMaxLength(32);
        modelBuilder.Entity<Author>().HasIndex(a => a.Email).IsUnique().HasFilter("[Email] <> ''");

        modelBuilder.Entity<Reaction>().HasIndex(r => new {r.CheepId, r.AuthorName}).IsUnique();
    }
    
}
