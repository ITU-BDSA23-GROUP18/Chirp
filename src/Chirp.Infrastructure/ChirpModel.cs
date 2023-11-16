using Chirp.core.DTOs;
using Microsoft.EntityFrameworkCore.Design;

namespace Chirp.Infrastructure;
public class ChirpDbContext : DbContext
{
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options) 
    {
    }
    
    public DbSet<Cheep>? Cheeps { get; set; }
    public DbSet<Author>? Authors { get; set; }
    
    
    public void InitializeDatabase(){
        Database.EnsureCreated();
        DbInitializer.SeedDatabase(this);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Cheep>().Property(c => c.Message).HasMaxLength(160);
        
        modelBuilder.Entity<Author>().Property(a => a.Name).HasMaxLength(32);
        modelBuilder.Entity<Author>().HasIndex(a => a.Name).IsUnique();
        modelBuilder.Entity<Author>().Property(a => a.Email).HasMaxLength(300);
        modelBuilder.Entity<Author>().HasIndex(a => a.Email).IsUnique().HasFilter("[Email] IS NOT NULL");
    }
}

public class Cheep
{
    public Guid CheepId { get; set; }
    public Guid AuthorId { get; set; }
    public required Author Author { get; set; }
    public required string Message { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class Author
{
    public Guid AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public List<Cheep>? Cheeps { get; set; }
    public List<Author>? Following { get; set; }
    public List<Author>? Followers { get; set; }
}

public class ChirpContextFactory : IDesignTimeDbContextFactory<ChirpDbContext>
    {
        public ChirpDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChirpDbContext>();
            var dbPath = Path.Combine(Path.GetTempPath(), "Chirp.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new ChirpDbContext(optionsBuilder.Options);
        }
    }




