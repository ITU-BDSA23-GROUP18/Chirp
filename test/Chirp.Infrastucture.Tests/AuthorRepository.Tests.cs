namespace Chirp.Infrastructure.Tests;

using Xunit;
using Microsoft.Data.Sqlite;

public class AuthorRepositoryTests
{
    private readonly ChirpContext _context;
    private readonly AuthorRepository _repository;

    public AuthorRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlite(connection);
        _context = new ChirpContext(builder.Options);
        _repository = new AuthorRepository(_context, false);
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
        Assert.Equal(name, author?.Name);

    }
    [Fact]
    public async void TestFindAuthorByEmail()
    {
        var a1 = new Author() { AuthorId = Guid.NewGuid(), Name = "Helge", Email = "ropf@itu.dk", Cheeps = new List<Cheep>() };
        var Authors = new List<Author>() { a1 };
        _context.Authors.AddRange(Authors);
        _context.SaveChanges();

        // Act
        var authors = await _repository.GetAuthorByEmail("ropf@itu.dk");
        var author = authors.FirstOrDefault();

        //Assert
        Assert.Equal("Helge", author?.Name);

    }

    [Fact]
    public async void TestCreateCheep()
    {
        // Act
        _repository.CreateAuthor("John Doe", "John@doe.com");

        var authors = await _repository.GetAuthorByName("John Doe");
        var author = authors.FirstOrDefault();
        //Assert
        Assert.Equal("John Doe", author?.Name);
    }

    [Fact]
    public async void TestFollowAuthor()
    {
        _repository.CreateAuthor("John Doe", "John@doe.com");
        _repository.CreateAuthor("Jane Doe", "Jane@doe.com");
        _repository.FollowAuthor("Jane Doe", "John Doe");
        var followerList = await _repository.GetFollowers("Jane Doe");
        var follower = followerList.First();
        Assert.Equal("John Doe", follower.Name);
    }
    [Fact]
    public async void TestGetFollowers()
    {
        _repository.CreateAuthor("John Doe", "John@doe.com");
        _repository.CreateAuthor("Jane Doe", "Jane@doe.com");
        _repository.CreateAuthor("Jack Doe", "Jack@doe.com");
        _repository.CreateAuthor("jill Doe", "jill@doe.com");
        _repository.FollowAuthor("John Doe", "Jane Doe");
        _repository.FollowAuthor("John Doe", "Jack Doe");
        _repository.FollowAuthor("John Doe", "jill Doe");
        var janeFolloweringList = await _repository.GetFollowing("Jane Doe");
        var jackFolloweringList = await _repository.GetFollowing("Jack Doe");
        var jillFolloweringList = await _repository.GetFollowing("jill Doe");
        Assert.Equal("John Doe", janeFolloweringList.First().Name);
        Assert.Equal("John Doe", jackFolloweringList.First().Name);
        Assert.Equal("John Doe", jillFolloweringList.First().Name);
    }
    
    [Fact]
    public async void TestGetFollowing()
    {
        _repository.CreateAuthor("John Doe", "John@doe.com");
        _repository.CreateAuthor("Jane Doe", "Jane@doe.com");
        _repository.CreateAuthor("Jack Doe", "Jack@doe.com");
        _repository.CreateAuthor("jill Doe", "jill@doe.com");
        _repository.FollowAuthor("Jane Doe", "John Doe");
        _repository.FollowAuthor("Jack Doe", "John Doe");
        _repository.FollowAuthor("jill Doe", "John Doe");
        var followerList = await _repository.GetFollowing("John Doe");
        foreach (var follower in followerList)
        {
            Assert.Contains(follower.Name, new List<string>() { "Jane Doe", "Jack Doe", "jill Doe" });
        }
    }
}
