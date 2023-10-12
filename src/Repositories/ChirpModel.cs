namespace Repositories;
public class ChirpContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public string? DbPath { get; }

    public ChirpContext() : base()
    {
        DbPath = Path.Combine(Path.GetTempPath(),"Chirp.db");
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
}

public class Cheep
{
    public int CheepId { get; set; }
    public required int AuthorId { get; set; }
    public required Author Author { get; set; }
    public required string Text { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class Author
{
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public List<Cheep> Cheeps { get; set;} = new ();
}
