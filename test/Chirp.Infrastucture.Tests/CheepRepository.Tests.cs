using Chirp.core.DTOs;
using Testcontainers.MsSql;

namespace Chirp.Infrastructure.Tests;

public class CheepRepositoryTests : IAsyncLifetime
{

    private readonly MsSqlContainer _msSqlContainer;

    public CheepRepositoryTests()
    {
        _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    }
    public async Task InitializeAsync()
    {

        await _msSqlContainer.StartAsync();
        var optionsBuilder = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        using var context = new ChirpDbContext(optionsBuilder.Options);
        await context.Database.MigrateAsync();

    }

    /*
    * Testing GetCheeps
    */

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetCheeps_returns32Cheeps(int page)
    {
        var builder = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpDbContext(builder.Options);
        var _repository = new CheepRepository(_context);
        var cheeps = await _repository.GetCheep(page);

        Assert.Equal(32, cheeps.Count());
    }

    [Fact]
    public async void GetCheeps_onFirstPage_returns32FirstCheeps()
    {
        var builder = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpDbContext(builder.Options);
        var _repository = new CheepRepository(_context);

        var cheeps = await _repository.GetCheep(); // page = 1

        var allCheeps = DbInitializer.Cheeps.Select(c => c.ToDTO());

        var cheepDtos = cheeps as CheepDTO[] ?? cheeps.ToArray();
        Assert.Equal(32, cheepDtos.Count());
        Assert.All(cheepDtos, c => Assert.Contains(c, allCheeps));

    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }

    [Fact]
    public async void GetCheeps_onAPageOutOfRange_returnsEmpty()
    {
        var builder = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpDbContext(builder.Options);
        var _repository = new CheepRepository(_context);

        var cheeps = await _repository.GetCheep(666);

        Assert.Empty(cheeps);
    }

    [Theory]
    [InlineData("Helge", "ropf@itu.dk")]
    [InlineData("Rasmus", "rnie@itu.dk")]
    public async void GetCheepsFromAuthor_givenAuthor_returnsOnlyCheepsByAuthor(string name, string email)
    {
        var builder = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpDbContext(builder.Options);
        var _repository = new CheepRepository(_context);

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
        var builder = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpDbContext(builder.Options);
        var _repository = new CheepRepository(_context);

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
        var builder = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpDbContext(builder.Options);
        var _repository = new CheepRepository(_context);
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
        var builder = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpDbContext(builder.Options);
        var _repository = new CheepRepository(_context);
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
        var builder = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpDbContext(builder.Options);
        var _repository = new CheepRepository(_context);
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
        var builder = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpDbContext(builder.Options);
        var _repository = new CheepRepository(_context);
        _repository.CreateCheep(message, authorName);
        Assert.Contains(_context.Authors, a => a.Name == authorName);
    }

    [Fact]
    public void ManyNewUsers_CanCreateCheeps_andReadCheep()
    {
        // 
        var builder = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpDbContext(builder.Options);
        var _repository = new CheepRepository(_context);

        List<CheepDTO> newCheeps = new Faker<CheepDTO>()
            .CustomInstantiator(f => new CheepDTO(f.Random.Words(), f.Name.FirstName(), f.Date.Recent().ToString("HH:mm:ss dd/MM/yyyy")))
            .RuleFor(c => c.Author, (f, c) => f.Name.FirstName())
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
