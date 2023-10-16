namespace Repositories;
public class ChirpContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public string? DbPath { get; }

    public ChirpContext() : base()
    {
        
    }
    
    public void InitializeDatabase(){
        //if the database is not created 
        if(!File.Exists(DbPath)){
            Database.EnsureCreated();
        }
        DbInitializer.SeedDatabase(this);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ensure the Name and Email property is unique
        modelBuilder.Entity<Author>().HasIndex(e => e.Name).IsUnique(); 
        modelBuilder.Entity<Author>().HasIndex(e => e.Email).IsUnique(); 
    }
}

public class Cheep
{
    public Guid CheepId { get; set; }
    public required Guid AuthorId { get; set; }
    public required Author Author { get; set; }
    public required string Text { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class Author
{
    public Guid AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public List<Cheep> Cheeps { get; set;} = new ();
}


