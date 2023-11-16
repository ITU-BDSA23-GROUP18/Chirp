using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure;
public class ChirpContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Reaction> Reactions { get; set; }

    public ChirpContext(DbContextOptions<ChirpContext> options) : base(options)
    {
        
    }
    
    public void InitializeDatabase(){
        Database.EnsureCreated();
        DbInitializer.SeedDatabase(this);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Cheep>().Property(c => c.Message).HasMaxLength(160);
        
        modelBuilder.Entity<Author>().Property(a => a.Name).HasMaxLength(32);
        modelBuilder.Entity<Author>().HasIndex(a => a.Name).IsUnique();
        modelBuilder.Entity<Author>().Property(a => a.Email).HasMaxLength(300);
        modelBuilder.Entity<Author>().HasIndex(a => a.Email).IsUnique();

        modelBuilder.Entity<Reaction>().HasIndex(r => new {r.CheepId, r.AuthorId}).IsUnique();
    }
}

