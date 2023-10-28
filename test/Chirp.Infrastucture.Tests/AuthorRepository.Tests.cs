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
        var authors = await _repository.GetAuthorByName("Helge");
        var author = authors.FirstOrDefault();
        
        // Assert
        Assert.Equal("Helge", author.Name);
        
    }
    [Fact]
    public async void TestFindAuthorByEmail(){

        // Act
        var authors = await _repository.GetAuthorByEmail("ropf@itu.dk");
        var author = authors.FirstOrDefault();

        //Assert
        Assert.Equal("Helge", author.Name);

    }

    [Fact]
    public async void TestCreateCheep(){

        // Act
        _repository.CreateAuthor("John Doe", "John@doe.com");

        var authors = await _repository.GetAuthorByName("John Doe");
        var author = authors.FirstOrDefault();
        //Assert
        Assert.Equal("Helge", author.Name);
    }
}
