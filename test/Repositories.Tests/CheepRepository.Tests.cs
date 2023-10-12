namespace Repositories.Tests;

public class CheepRepositories
{
    private readonly ICheepRepository _repository;

    public CheepRepositoryTests()
    {
        new MainCheepDTO("Helge","Hello, BDSA students!",UnixTimeStampToDateTimeString(1690892208)),
        new MainCheepDTO("Rasmus","Hello, BDSA students!",UnixTimeStampToDateTimeString(1690892208))
    };
    
    
    public CheepRepository CreateInMemoryDatabase() {
        // Arrange
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<CheepContext>().UseSqlite(connection);
        using var context = new CheepContext(builder.Options);
        context.Database.EnsureCreated();
        _repository = new CheepRepository(context);
        // Seed databse
        var author1 = new Author {Name = "Helge", Email = "ropf@itu.dk"};
        var author2 = new Author { Name = "OndFisk", Email = "rasmus@microsoft.com" };
        context.Add(new Cheep {Author = author1, Text = "Hello everybody!"});
        context.Add(new Cheep {Author = author1, Text = "Welcome to the course ;)"});
        context.Add(new Cheep {Author = author2, Text = "Write clean code!"});
        context.Add(new Cheep {Author = author2, Text = "I like VS Code <3"});
        context.SaveChanges();
    }

    [Fact]
    public async void GetCheeps_returnsThirtyTwoCheepsFromFirstPage()
    {
        var cheeps = await  _cheepService.GetCheep(0);
    
        Assert.Equal(32, cheeps.Count());

    }
    
    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    public async void GetCheepsFromAuthor_givenAuthor_returnsOnlyCheepsByAuthor(string author)
    {
        Author authorObject = new Author{
            Name = $"{author}",
            Email = $"{author}@gmail.com"
        };

        var cheeps = await _cheepService.GetCheepFromAuthor(authorObject, 0);
        
        Assert.Contains(_cheeps.Find(c => c.Author == author), cheeps);
        Assert.DoesNotContain(_cheeps.Find(c => c.Author != author), cheeps);
    }
    
    [Theory]
    [InlineData("OndFisk")]
    public async void GetCheepsFromAuthor_givenNonExistingAuthor_returnsEmpty(string author)
    {
         Author authorObject = new Author{
            Name = $"{author}",
            Email = $"{author}@gmail.com"
        };


        var cheeps = await _cheepService.GetCheepFromAuthor(authorObject, 0);
        Assert.Empty(cheeps);
    }
    
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}
