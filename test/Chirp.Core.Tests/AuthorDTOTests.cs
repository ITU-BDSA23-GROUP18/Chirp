namespace Chirp.Core.Tests;

public class AuthorDTOTests
{
    [Theory]
    [InlineData("test1", "test2@test.com", "", "")]
    [InlineData("test2", "test3@test.com", "", "")]
    public void CreateAuthorDTOTest(string name, string email, string profilePictureUrl, string displayName)
    {
        var authorDTO = new AuthorDTO(name, email, profilePictureUrl, displayName);

        Assert.Equal(authorDTO.Name, authorDTO.Name);
        Assert.Equal(authorDTO.Email, authorDTO.Email);
    }

    [Theory]
    [InlineData(null, "m", "", "")]
    [InlineData("", "m", "", "")]
    public void AuthorDTONullValueTest(string? name, string? email, string? profilePictureUrl, string displayName)
    {
        var exceptionType = typeof(ArgumentNullException);

        Assert.Throws(exceptionType, () =>
        {
            var AuthorDTO = new AuthorDTO(name!, email!, profilePictureUrl!, displayName);
        });
    }
}
