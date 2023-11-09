namespace Chirp.Infrastructure.Tests;

using Testcontainers.MsSql;
public class AuthorRepositoryTests : IAsyncLifetime
{


    private readonly MsSqlContainer _msSqlContainer;
    public AuthorRepositoryTests()
    {
        // Arrange
        _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

        
    }

    public async Task InitializeAsync()
    {

        await _msSqlContainer.StartAsync();
        var optionsBuilder = new DbContextOptionsBuilder<ChirpContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        using var context = new ChirpContext(optionsBuilder.Options);
        await context.Database.MigrateAsync();

    }
    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
    
    [Fact]
    public async void TestFindAuthorByName()
    {

        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpContext(builder.Options);
        var _repository = new AuthorRepository(_context);
        // Act
        var authors = await _repository.GetAuthorByName("Helge");
        var author = new AuthorDTO(null,null);

        for (int i = 0; i < authors.Count(); i++)
        {
            author = authors.ElementAt(0);
            // Assert
        }
        
        // Assert
        Assert.Equal("Helge", author.Name);
        
    }
    [Fact]
    public async void TestFindAuthorByEmail(){

        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpContext(builder.Options);
        var _repository = new AuthorRepository(_context);
        // Act
        var authors = await _repository.GetAuthorByEmail("ropf@itu.dk");
        var author = new AuthorDTO(null,null);

        for (int i = 0; i < authors.Count(); i++)
        {
            author = authors.ElementAt(0);
            // Assert
        }

        //Assert
        Assert.Equal("Helge", author.Name);

    }

    [Fact]
    public async void TestCreateCheep(){

        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpContext(builder.Options);
        var _repository = new AuthorRepository(_context);
        // Act
        _repository.CreateAuthor("John Doe", "John@doe.com");

        var authors = await _repository.GetAuthorByName("John Doe");
        var author = authors.FirstOrDefault();
        //Assert
        Assert.Equal("John Doe", author.Name);
    }

    
}
