using Microsoft.Data.Sqlite;

namespace Chirp.Infrastructure.Tests;

// Using same repository in 2 test files run in parallel, even if it is a in-memory database,
// can cause concurrency issues, sometimes. So, we need to run the tests sequentially.
[Collection("Cheep Repository Collection")]
public class CheepRepositoryTests
{
    private readonly ChirpContext _context;
    private readonly CheepRepository _repository;

    public CheepRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlite(connection);
        _context = new ChirpContext(builder.Options);
        _repository = new CheepRepository(_context, false);
    }

    public class CheepDTOComparer : IEqualityComparer<CheepDTO>
    {
        public bool Equals(CheepDTO? x, CheepDTO? y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;
            return x.Author == y.Author && x.Message == y.Message && x.Timestamp == y.Timestamp;
        }

        public int GetHashCode(CheepDTO obj) => obj.GetHashCode();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetCheeps_returns32Cheeps(int page)
    {
        _context.InitializeDatabase(true);

        var cheeps = await _repository.GetCheep(page);

        Assert.Equal(32, cheeps.Count());
    }

    [Fact]
    public async void GetCheeps_onFirstPage_returns32FirstCheeps()
    {
        _context.InitializeDatabase(true);

        var cheeps = await _repository.GetCheep(); // page = 1

        var allCheeps = DbInitializer.Cheeps.Select(c => c.ToDTO());

        Assert.Equal(32, cheeps.Count());
        Assert.All(cheeps, c => Assert.Contains(c, allCheeps, new CheepDTOComparer()));
    }

    [Fact]
    public async void GetCheeps_onAPageOutOfRange_returnsEmpty()
    {
        _context.InitializeDatabase(true);

        var cheeps = await _repository.GetCheep(666);

        Assert.Empty(cheeps);
    }

    [Theory]
    [InlineData("Helge", "ropf@itu.dk")]
    [InlineData("Rasmus", "rnie@itu.dk")]
    public async void GetCheepsFromAuthor_givenAuthor_returnsOnlyCheepsByAuthor(string name, string email)
    {
        _context.InitializeDatabase(true);

        var author = new Author
        {
            Name = name,
            Email = email,
        };

        var cheeps = await _repository.GetCheepFromAuthor(author.Name);

        var aCheeps = new List<CheepDTO>();
        foreach (var c in DbInitializer.Cheeps.Where(c => c.Author.Name == author.Name).Take(32))
        {
            aCheeps.Add(c.ToDTO());
        }

        Assert.All(cheeps, c => Assert.Contains(c, aCheeps, new CheepDTOComparer()));
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", "Jacqualine.Gilcoine@gmail.com", 1)]
    [InlineData("Jacqualine Gilcoine", "Jacqualine.Gilcoine@gmail.com", 2)]

    public async void GetCheepsFromAuthor_givenAuthorAndPage_returns32Cheeps(string name, string email, int page)
    {
        _context.InitializeDatabase(true);

        var author = new Author
        {
            Name = name,
            Email = email,
        };

        var cheeps = await _repository.GetCheepFromAuthor(author.Name, page);
        Assert.Equal(32, cheeps.Count());
    }

    [Fact]
    public async void GetCheepsFromAuthor_givenNonExistingAuthor_returnsEmpty()
    {
        _context.InitializeDatabase(true);

        var author = new Author
        {
            Name = "OndFisk",
            Email = "rasmus@microsoft.com",
        };

        var cheeps = await _repository.GetCheepFromAuthor(author.Name);

        Assert.Empty(cheeps);
    }

    [Fact]
    public async void GetCheepsFromAuthor_onAPageOutOfRange_returnsEmpty()
    {
        var author = new Author
        {
            Name = "Helge",
            Email = "ropf@itu.dk",
        };

        var cheeps = await _repository.GetCheepFromAuthor(author.Name, 666);

        Assert.Empty(cheeps);
    }

    // Testing CreateCheep
    [Theory]
    [InlineData("Hello my name is Helge", "Helge")]
    [InlineData("I work at Microsoft", "Rasmus")]
    public void CreateCheep_givenCheepWithAuthor_savesThatCheep(string message, string authorName)
    {
        _context.InitializeDatabase(true);

        var author = _context.Authors.First(a => a.Name == authorName);

        _repository.CreateCheep(message, authorName);

        var cheeps = _context.Cheeps;
        Assert.Contains(cheeps, c => c.Message == message && c.Author == author);
    }

    [Theory]
    [InlineData("I love coding <3", "OndFisk")]
    [InlineData("I can walk non water!", "Jesus")]
    public void CreateCheep_givenCheepWithNonExistingAuthor_CreatesAuthor(string message, string authorName)
    {
        _repository.CreateCheep(message, authorName);
        Assert.Contains(_context.Authors, a => a.Name == authorName);
    }

    [Fact]
    public void ManyNewUsers_CanCreateCheeps_andReadCheep()
    {
        // Use GUID as username since the username must be uniqe
        List<CheepDTO> newCheeps = new Faker<CheepDTO>()
            .CustomInstantiator(f => new CheepDTO(Guid.NewGuid().ToString()[4..], f.Random.Words(), f.Date.Recent().ToString("HH:mm:ss dd/MM/yyyy"), f.Date.Recent().ToString("HH:mm:ss dd/MM/yyyy"), new List<ReactionDTO>(), "", ""))
            .RuleFor(c => c.Message, (f, c) => f.Random.Words())
            .RuleFor(c => c.Timestamp, (f, c) => f.Date.Recent().ToString("HH:mm:ss dd/MM/yyyy"))
            .GenerateBetween(50, 100);

        foreach (var cheep in newCheeps)
        {
            _repository.CreateCheep(cheep.Message, cheep.Author);
        }

        Assert.AllAsync(newCheeps, async (c) =>
        {
            var cheeps = await _repository.GetCheepFromAuthor(c.Author);
            Assert.Contains(c, cheeps, new CheepDTOComparer());
        });
    }
}
