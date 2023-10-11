namespace Chirp.Razor.Tests;
using Repositories;
using Repositories.DTO;
public class CheepServiceTests
{
    private readonly IRepository<Cheep, MainCheepDTO, Author> _cheepService;
    private static readonly List<MainCheepDTO> _cheeps = new()
    {
        new MainCheepDTO("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
        new MainCheepDTO("Rasmus", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
    };
    
    public CheepServiceTests()
    {
        _cheepService = new CheepRepository();
    }

    [Fact]
    public void GetCheeps_returnsThirtyTwoCheepsFromFirstPage()
    {
        var cheeps = _cheepService.Get(0);
    
        Assert.Equal(32, cheeps.Count);

    }
    
    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    public void GetCheepsFromAuthor_givenAuthor_returnsOnlyCheepsByAuthor(string author)
    {
        var cheeps = _cheepService.GetFrom(author, 0);
        
        Assert.Contains(_cheeps.Find(c => c.Author == author), cheeps);
        Assert.DoesNotContain(_cheeps.Find(c => c.Author != author), cheeps);
    }
    
    [Theory]
    [InlineData("OndFisk")]
    public void GetCheepsFromAuthor_givenNonExistingAuthor_returnsEmpty(string author)
    {
        var cheeps = _cheepService.GetFrom(author, 0);
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
