namespace Chirp.Infrastructure.Tests;

using Testcontainers.MsSql;
using Xunit;

using Microsoft.Data.Sqlite;
using Chirp.Infrastucture;

public class AuthorRepositoryTests
{
    private readonly ChirpContext _context;
    private readonly AuthorRepository _repository;


    public AuthorRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlite(connection);
        using var _context = new ChirpContext(builder.Options);
        var _repository = new AuthorRepository(_context);

    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Roger")]
    public async void TestFindAuthorByName(string name)
    {


        var a1 = new Author() { AuthorId = Guid.NewGuid(), Name = name, Email = "Roger+Histand@hotmail.com", Cheeps = new List<Cheep>() };
        var Authors = new List<Author>() { a1 };
        _context.Authors.AddRange(Authors);
        _context.SaveChanges();
        // Act
        var authors = await _repository.GetAuthorByName(name);
        var author = authors.FirstOrDefault();
        // Assert
        Assert.Equal(name, author.Name);

    }
    [Fact]
    public async void TestFindAuthorByEmail()
    {

        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlite(connection);
        using var _context = new ChirpContext(builder.Options);
        var _repository = new AuthorRepository(_context);

        var a1 = new Author() { AuthorId = Guid.NewGuid(), Name = "Helge", Email = "ropf@itu.dk", Cheeps = new List<Cheep>() };
        var Authors = new List<Author>() { a1 };
        _context.Authors.AddRange(Authors);
        _context.SaveChanges();
        


        // Act
        var authors = await _repository.GetAuthorByEmail("ropf@itu.dk");
        var author = authors.FirstOrDefault();

        //Assert
        Assert.Equal("Helge", author.Name);

    }

    [Fact]
    public async void TestCreateCheep()
    {

        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlite(connection);
        using var _context = new ChirpContext(builder.Options);
        var _repository = new AuthorRepository(_context);
        // Act
        _repository.CreateAuthor("John Doe", "John@doe.com");

        var authors = await _repository.GetAuthorByName("John Doe");
        var author = authors.FirstOrDefault();
        //Assert
        Assert.Equal("John Doe", author.Name);
    }
}
