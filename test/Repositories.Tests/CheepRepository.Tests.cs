using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Tests;

public class CheepRepositoryTests
{

    public CheepRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory");
        connection.Open();
        var builder = new DbContextOptionsBuilder<CheepContext>().UseSqlite(connection);
        using var context = new CheepContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        //var repository = new CheepRepository(context);
    }
    
    [Fact]
    public void Test1()
    {

    }
}
