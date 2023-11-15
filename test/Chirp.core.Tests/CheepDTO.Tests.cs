using System.Diagnostics;

namespace Chirp.core.Tests;

public class CheepDTOTests
{
    [Theory]
    [InlineData(null,null,null)]
    [InlineData("test1", null, null)]
    [InlineData("", "test2",null)]
    [InlineData("test1","test2","test3")]
    public void CreateCheepDTOTest(string author, string message, string timestamp)
    {
        
        var cheepDTO= new CheepDTO(author, message, timestamp);

        Assert.Equal(cheepDTO.Author, cheepDTO.Author);
        Assert.Equal(cheepDTO.Message, cheepDTO.Timestamp);
        Assert.Equal(cheepDTO.Timestamp, cheepDTO.Timestamp);
    }
}