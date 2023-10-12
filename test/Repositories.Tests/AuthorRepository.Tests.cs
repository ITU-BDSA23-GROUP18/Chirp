namespace Repositories.Tests;

public class AuthorRepositoryTests
{

    public AuthorRepositoryTests()
    {
        // Arrange


        
    }
    
    [Fact]
    public async void TestFindAuthorByName()
    {
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<CheepContext>().UseSqlite(connection);
        using var context = new CheepContext(builder.Options);
        var repository = new AuthorRepository(context);

        // Act
        Author author = await repository.GetAuthorByName("Helge");
        
        // Assert
        Assert.Equal("Helge", author.Name);
        connection.Close();
    }
    [Fact]
    public async void TestFindAuthorByEmail(){
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<CheepContext>().UseSqlite(connection);
        using var context = new CheepContext(builder.Options);
        var repository = new AuthorRepository(context);
        // Act
        Author author = await repository.GetAuthorByEmail("ropf@itu.dk");

        //Assert
        Assert.Equal("Helge", author.Name);
        connection.Close();

    }

    [Fact]
    public async void TestCreateCheep(){
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<CheepContext>().UseSqlite(connection);
        using var context = new CheepContext(builder.Options);
        var repository = new AuthorRepository(context);h
        // Act
        await repository.CreateAuthor("Helge");

        Author author = await repository.GetAuthorByName("Helge");

        //Assert
        Assert.Equal("Helge", author.Name);
        connection.Close();
    }
}
