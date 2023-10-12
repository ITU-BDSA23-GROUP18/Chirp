namespace Repositories.Tests;

public class CheepRepositoryTests
{
    private readonly ICheepRepository _repository;
    
    public CheepRepositoryTests()
    {   
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<CheepContext>().UseSqlite(connection);
        var context = new CheepContext(builder.Options);
        context.Database.EnsureCreated();
        _repository = new CheepRepository(context);
        // Seed database
        DbInitializer.SeedDatabase(context);
        context.SaveChanges();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async void GetCheeps_returns32Cheeps(int page)
    {
        var cheeps = await  _repository.GetCheep(page);
    
        Assert.Equal(32, cheeps.Count());
    }
    
    [Fact]
    public async void GetCheeps_onFirstPage_returns32FirstCheeps()
    {
        var cheeps = await  _repository.GetCheep(); // page = 1

        var first32Cheeps = new List<MainCheepDTO>();
        foreach (var c in DbInitializer.Cheeps.Take(32))
        {
            first32Cheeps.Add(new MainCheepDTO(c.Author.Name, c.Text, c.TimeStamp.ShowString()));
        }
        Assert.All(cheeps, c => Assert.Contains(c, first32Cheeps));
    }
    
    [Fact]
    public async void GetCheeps_onAPageOutOfRange_returnsEmpty()
    {
        var cheeps = await  _repository.GetCheep(666);
    
        Assert.Empty(cheeps);
    }
    
    [Theory]
    [InlineData("Helge", "ropf@itu.dk")]
    [InlineData("Rasmus", "rnie@itu.dk")]
    public async void GetCheepsFromAuthor_givenAuthor_returnsOnlyCheepsByAuthor(string name, string email)
    {
        var author = new Author{
            Name = name,
            Email = email
        };

        var cheeps = await _repository.GetCheepFromAuthor(author);

        var aCheeps = new List<MainCheepDTO>();
        foreach (var c in DbInitializer.Cheeps.Where(c => c.Author.Name == author.Name).Take(32))
        {
            aCheeps.Add(new MainCheepDTO(c.Author.Name, c.Text, c.TimeStamp.ShowString()));
        }
        Assert.All(cheeps, c => Assert.Contains(c, aCheeps));
    }
    
    [Theory]
    [InlineData("Jacqualine Gilcoine", "Jacqualine.Gilcoine@gmail.com", 1)]
    [InlineData("Jacqualine Gilcoine", "Jacqualine.Gilcoine@gmail.com", 2)]

    public async void GetCheepsFromAuthor_givenAuthorAndPage_returns32Cheeps(string name, string email, int page)
    {
        var author = new Author{
            Name = name,
            Email = email
        };

        var cheeps = await _repository.GetCheepFromAuthor(author);

        Assert.Equal(32, cheeps.Count());

    }
    
    [Fact]
    public async void GetCheepsFromAuthor_givenNonExistingAuthor_returnsEmpty()
    {
         var author = new Author{
            Name = "OndFisk",
            Email = "rasmus@microsoft.com"
         };
         
        var cheeps = await _repository.GetCheepFromAuthor(author);
        
        Assert.Empty(cheeps);
    }
    
    [Fact]
    public async void GetCheepsFromAuthor_onAPageOutOfRange_returnsEmpty()
    {
        var author = new Author{
            Name = "Helge",
            Email = "ropf@itu.dk"
        };
        
        var cheeps = await  _repository.GetCheepFromAuthor(author, 666);
    
        Assert.Empty(cheeps);
    }
}
