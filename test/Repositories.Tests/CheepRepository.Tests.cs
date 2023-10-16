namespace Repositories.Tests;

public class CheepRepositoryTests
{
    private readonly ICheepRepository _repository;
    private readonly CheepContext _context;
    
    public CheepRepositoryTests()
    {   
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<CheepContext>().UseSqlite(connection);
        _context = new CheepContext(builder.Options);
        _context.Database.EnsureCreated();
        _repository = new CheepRepository(_context);
        // Seed database
        DbInitializer.SeedDatabase(_context);
        _context.SaveChanges();
    }
    
    /*
     * Testing GetCheeps
     */

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
    
    /*
     * Testing GetCheepsFromAuthor
     */
    
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
    
    /*
     * Testing CreateCheep
     */
    
    [Theory]
    [InlineData("Hello my name is Helge", "Helge")]
    [InlineData("I work at Microsoft", "Rasmus")]
    public async void CreateCheep_givenCheepAndWithAuthor_savesThatCheep(string message, string author)
    {
        var time = DateTime.Now;
        var cDto = new MainCheepDTO(author, message, time.ShowString());
        var c = new Cheep
        {
            Text = message,
            Author = _context.Authors.First(a => a.Name == author),
            TimeStamp = time
        };

        await  _repository.CreateCheep(cDto);

        var cheeps = _context.Cheeps;
        Assert.Contains(c, cheeps);
    }
    
    [Theory]
    [InlineData("I love coding <3", "OndFisk")]
    [InlineData("I can walk non water!", "Jesus")]
    public async void CreateCheep_givenCheepWithNonExistingAuthor_savesThatCheepAndAuthor(string message, string author)
    {
        var time = DateTime.Now;
        var cheep = new MainCheepDTO(author, message, time.ShowString());
        var c = new Cheep
        {
            Text = message,
            Author = _context.Authors.First(a => a.Name == author),
            TimeStamp = time
        };
        
        var cheeps = await  _repository.CreateCheep(cheep);
    
        Assert.Empty(cheeps);
    }
}
