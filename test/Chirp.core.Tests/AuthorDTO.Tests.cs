using System.Diagnostics;

namespace Chirp.core.Tests;

public class authorDTOTests
{
    [Theory]
    [InlineData("test1", null)]
    [InlineData("", "test2")]
    public void CreateAuthorDTOTest(string name, string email)
    {
        
        var authorDTO= new AuthorDTO(name, email);

        Assert.Equal(authorDTO.Name, authorDTO.Name);
        Assert.Equal(authorDTO.Email, authorDTO.Email);
    }

}