using Testcontainers.MsSql;

namespace Chirp.Infrastructure.Tests;

public class CheepRepositoryTests : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();
    private ChirpContext _context;
    private CheepRepository _repository;

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        _context = new ChirpContext(builder.Options);
        _repository = new CheepRepository(_context);
    }

    public Task DisposeAsync()
    {
        return _msSqlContainer.DisposeAsync().AsTask();
    }
    /*
    * Testing GetCheeps
    */

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetCheeps_returns32Cheeps(int page)
    {
        var cheeps = await _repository.GetCheep(page);

        Assert.Equal(32, cheeps.Count());
    }

    [Fact]
    public async void GetCheeps_onFirstPage_returns32FirstCheeps()
    {
        var cheeps = await _repository.GetCheep(); // page = 1

        var allCheeps = DbInitializer.Cheeps.Select(c => c.ToDTO());

        var cheepDtos = cheeps as CheepDTO[] ?? cheeps.ToArray();
        Assert.Equal(32, cheepDtos.Count());
        Assert.All(cheepDtos, c => Assert.Contains(c, allCheeps));
    }

    [Fact]
    public async void GetCheeps_onAPageOutOfRange_returnsEmpty()
    {
        var cheeps = await _repository.GetCheep(666);

        Assert.Empty(cheeps);
    }

    [Theory]
    [InlineData("Helge", "ropf@itu.dk")]
    [InlineData("Rasmus", "rnie@itu.dk")]
    public async void GetCheepsFromAuthor_givenAuthor_returnsOnlyCheepsByAuthor(string name, string email)
    {
        var author = new Author
        {
            Name = name,
            Email = email
        };

        var cheeps = await _repository.GetCheepFromAuthor(author.Name);

        var aCheeps = new List<CheepDTO>();
        foreach (var c in DbInitializer.Cheeps.Where(c => c.Author.Name == author.Name).Take(32))
        {
            aCheeps.Add(c.ToDTO());
        }
        Assert.All(cheeps, c => Assert.Contains(c, aCheeps));
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", "Jacqualine.Gilcoine@gmail.com", 1)]
    [InlineData("Jacqualine Gilcoine", "Jacqualine.Gilcoine@gmail.com", 2)]

    public async void GetCheepsFromAuthor_givenAuthorAndPage_returns32Cheeps(string name, string email, int page)
    {
        var author = new Author
        {
            Name = name,
            Email = email
        };

        var cheeps = await _repository.GetCheepFromAuthor(author.Name, page);

        Assert.Equal(32, cheeps.Count());
    }

    [Fact]
    public async void GetCheepsFromAuthor_givenNonExistingAuthor_returnsEmpty()
    {
        var author = new Author
        {
            Name = "OndFisk",
            Email = "rasmus@microsoft.com"
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
            Email = "ropf@itu.dk"
        };

        var cheeps = await _repository.GetCheepFromAuthor(author.Name, 666);

        Assert.Empty(cheeps);
    }

    //Testing CreateCheep

    [Theory]
    [InlineData("Hello my name is Helge", "Helge")]
    [InlineData("I work at Microsoft", "Rasmus")]
    public void CreateCheep_givenCheepWithAuthor_savesThatCheep(string message, string authorName)
    {
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
        //Use GUID as username since the username must be uniqe
        List<CheepDTO> newCheeps = new Faker<CheepDTO>()
            .CustomInstantiator(f => new CheepDTO(Guid.NewGuid().ToString()[4..], f.Random.Words(), f.Date.Recent().ToString("HH:mm:ss dd/MM/yyyy"), new List<ReactionDTO>()))
            .RuleFor(c => c.Message, (f, c) => f.Random.Words())
            .RuleFor(c => c.Timestamp, (f, c) => f.Date.Recent().ToString("HH:mm:ss dd/MM/yyyy"))
            .GenerateBetween(50, 100);

        foreach (var cheep in newCheeps)
        {
            _repository.CreateCheep(cheep.Message, cheep.Author);
        }

        Assert.AllAsync<CheepDTO>(newCheeps, async (c) =>
        {
            var cheeps = await _repository.GetCheepFromAuthor(c.Author);
            Assert.Contains(c, cheeps);
        });
    }
}
