namespace Chirp.Razor.Tests;

public class CheepServiceTests
{
    private readonly ICheepService _cheepService;
    private static readonly List<CheepViewModel> _cheeps = new()
    {
        new CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
        new CheepViewModel("Rasmus", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
    };
    
    public CheepServiceTests()
    {
        _cheepService = new CheepService();
    }

    [Fact]
    public void GetCheeps_returnsAllCheeps()
    {
        var cheeps = _cheepService.GetCheeps(0);
        
        Assert.Contains(_cheeps[0], cheeps);
        Assert.Contains(_cheeps[1], cheeps);

    }
    
    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    public void GetCheepsFromAuthor_givenAuthor_returnsOnlyCheepsByAuthor(string author)
    {
        var cheeps = _cheepService.GetCheepsFromAuthor(author, 0);
        
        Assert.Contains(_cheeps.Find(c => c.Author == author), cheeps);
        Assert.DoesNotContain(_cheeps.Find(c => c.Author != author), cheeps);
    }
    
    [Theory]
    [InlineData("OndFisk")]
    public void GetCheepsFromAuthor_givenNonExistingAuthor_returnsEmpty(string author)
    {
        var cheeps = _cheepService.GetCheepsFromAuthor(author, 0);
        
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
