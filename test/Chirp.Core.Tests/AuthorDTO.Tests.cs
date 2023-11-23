namespace Chirp.Core.Tests;

public class authorDTOTests
{
    [Theory]
    [InlineData("test1", "test2@test.com")]
    [InlineData("test2", "test3@test.com")]
    public void CreateAuthorDTOTest(string name, string email)
    {
        var authorDTO = new AuthorDTO(name, email);

        Assert.Equal(authorDTO.Name, authorDTO.Name);
        Assert.Equal(authorDTO.Email, authorDTO.Email);
    }

    [Theory]
    [InlineData(null, "m")]
    [InlineData("", "m")]
    [InlineData("name", null)]
    [InlineData("name", "")]
    public void AuthorDTONullValueTest(string? name, string? email)
    {
        var exceptionType = typeof(ArgumentNullException);

        Assert.Throws(exceptionType, () =>
        {
            var CheepDTO = new AuthorDTO(name!, email!);
        });
    }
}
