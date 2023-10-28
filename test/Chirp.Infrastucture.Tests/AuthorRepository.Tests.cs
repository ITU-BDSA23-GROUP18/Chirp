namespace Chirp.Infrastructure.Tests;
public class AuthorRepositoryTests
{

    private readonly IAuthorRepository _repository;
    private readonly ChirpContext _context;
    public AuthorRepositoryTests()
    {
        // Arrange
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpContext>().UseSqlite(connection);
        _context = new ChirpContext(builder.Options);
        _context.InitializeDatabase();

        _repository = new AuthorRepository(_context);

        
    }
    
    [Fact]
    public async void TestFindAuthorByName()
    {

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
