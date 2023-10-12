using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Tests;

public class CheepRepositoryTests
{
    private readonly ICheepRepository _repository;

    public CheepRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory");
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
    public void Test1()
    {
        
    }
}
