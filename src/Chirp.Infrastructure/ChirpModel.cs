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
        modelBuilder.Entity<Author>().HasIndex(a => a.Email).IsUnique();

        modelBuilder.Entity<Reaction>().HasIndex(r => r.CheepId).IsUnique();
    }

    /*public class ChirpContextFactory : IDesignTimeDbContextFactory<ChirpContext>
    {
        public ChirpContext CreateContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChirpContext>();
            var dbPath = Path.Combine(Path.GetTempPath(), "Chirp.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new ChirpContext(optionsBuilder.Options);
        }
    }*/
}
