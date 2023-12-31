namespace Chirp.Core.Tests;

public class CheepDTOTests
{
    [Theory]
    [InlineData("test1", "test2", "test3")]
    [InlineData("test2", "test3", "test4")]
    [InlineData("test3", "test4", "test5")]
    public void CreateCheepDTOTest(string author, string message, string timestamp)
    {
        List<ReactionDTO> reactions = new List<ReactionDTO>();
        var cheepDTO = new CheepDTO("", author, message, timestamp, reactions, author);

        Assert.Equal(cheepDTO.Author, cheepDTO.Author);
        Assert.Equal(cheepDTO.Message, cheepDTO.Message);
        Assert.Equal(cheepDTO.Timestamp, cheepDTO.Timestamp);
    }

    [Theory]
    [InlineData(null, "test2", "test3", null)]
    [InlineData("test2", null, "test4", null)]
    [InlineData("test3", "test4", null, null)]
    [InlineData("", "test2", "test3", null)]
    [InlineData("test2", "", "test4", null)]
    [InlineData("test3", "test4", "", null)]
    public void NullValuesInCheepDTOTest(string? author, string? message, string? timestamp, List<ReactionDTO>? reactions)
    {
        var exceptionType = typeof(ArgumentNullException);

        Assert.Throws(exceptionType, () =>
        {
            var cheepDTO = new CheepDTO("", author!, message!, timestamp!, reactions!, author!);
        });
    }
}
