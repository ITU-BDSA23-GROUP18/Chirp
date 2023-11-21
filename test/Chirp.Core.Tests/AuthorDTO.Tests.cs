using System.Diagnostics;

namespace Chirp.Core.Tests;

public class authorDTOTests
{
    [Theory]
    [InlineData("test1", null)]
    [InlineData("test2", "test3")]
    public void CreateAuthorDTOTest(string name, string email)
    {
        var authorDTO= new AuthorDTO(name, email);

        Assert.Equal(authorDTO.Name, authorDTO.Name);
        Assert.Equal(authorDTO.Email, authorDTO.Email);
    }
    
    [Theory]
    [InlineData(null,"m")]
    [InlineData("","m")]
    public void AuthorDTONullValueTest(string name, string email){
        var exceptionType = typeof(ArgumentNullException);
        
        Assert.Throws(exceptionType, () => {
            var CheepDTO = new AuthorDTO(name, email);
        });
    }
}
