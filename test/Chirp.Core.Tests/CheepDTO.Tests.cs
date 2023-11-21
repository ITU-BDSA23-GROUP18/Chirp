using System.Diagnostics;

namespace Chirp.Core.Tests;

public class CheepDTOTests
{
    [Theory]
    [InlineData("test1","test2","test3")]
    [InlineData("test2","test3","test4")]
    [InlineData("test3","test4","test5")]
    public void CreateCheepDTOTest(string author, string message, string timestamp)
    {
        var cheepDTO= new CheepDTO(author, message, timestamp);

        Assert.Equal(cheepDTO.Author, cheepDTO.Author);
        Assert.Equal(cheepDTO.Message, cheepDTO.Message);
        Assert.Equal(cheepDTO.Timestamp, cheepDTO.Timestamp);
    }
    [Theory]
    [InlineData(null,"test2","test3")]
    [InlineData("test2",null,"test4")]
    [InlineData("test3","test4",null)]
    [InlineData("","test2","test3")]
    [InlineData("test2","","test4")]
    [InlineData("test3","test4","")]
    public void NullValuesInCheepDTOTest(string author, string message, string timestamp){
        
        var exceptionType = typeof(ArgumentNullException);
        
        Assert.Throws(exceptionType, () => {
            var CheepDTO = new CheepDTO(author, message, timestamp);
        });
    }
}
