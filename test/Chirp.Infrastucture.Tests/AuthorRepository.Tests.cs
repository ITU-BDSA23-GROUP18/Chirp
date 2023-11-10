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

    [Theory]
    [InlineData("Helge")]
    [InlineData("Roger")]
    public async void TestFindAuthorByName(string name)
    {
        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpContext(builder.Options);
        var _repository = new AuthorRepository(_context);

        var a1 = new Author() { AuthorId = Guid.NewGuid(), Name = name, Email = "Roger+Histand@hotmail.com", Cheeps = new List<Cheep>() };
        var Authors = new List<Author>() { a1 };
        _context.Authors.AddRange(Authors);
        _context.SaveChanges();
        // Act
        var authors = await _repository.GetAuthorByName(name);
        var author = authors.FirstOrDefault();
        // Assert
        Assert.Equal(name, author.Name);

    }/*
    [Fact]
    public async void TestFindAuthorByEmail()
    {

        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlServer(_msSqlContainer.GetConnectionString());
        var _context = new ChirpContext(builder.Options);
        var _repository = new AuthorRepository(_context);
        // Act
        var authors = await _repository.GetAuthorByEmail("ropf@itu.dk");
        var author = authors.FirstOrDefault();

        //Assert
        Assert.Equal("Helge", author.Name);

    }

    [Fact]
    public async void TestCreateCheep()
    {

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
*/

}
