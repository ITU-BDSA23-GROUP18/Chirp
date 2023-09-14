using Xunit.Abstractions;

namespace SimpleDB.Tests;

public record Cheep(string Author, string Message, long Timestamp);

public class ReadAndWriteCheep
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Cheep _cheep1 = new Cheep(Author: "ropf", Message: "Hello, BDSA students!", Timestamp: 1690891760);
    private readonly Cheep _cheep2 = new Cheep("rnie","Welcome to the course!",1690978778);
    private readonly Cheep _cheep3 = new Cheep("rnie","I hope you had a good summer.",1690979858);
    private readonly Cheep _cheep4 = new Cheep("ropf","Cheeping cheeps on Chirp :)",1690981487);
    private readonly Cheep _cheepTest = new Cheep("John Doe", "Testing...123", 1690991457);

    public ReadAndWriteCheep(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    [Fact]
    public void Read_All_Cheeps_From_Database()
    {
        var controller = new CSVDatabase<Cheep>();
        var actual = controller.Read();
        
        var expected = new List<Cheep>(){_cheep1, _cheep2, _cheep3, _cheep4};

        Assert.Equal(expected, actual);

    }

    [Fact]
    public void Delete_Cheep_From_Database()
    {
        var controller = new CSVDatabase<Cheep>();
        controller.Delete(_cheep4);

        var actual = controller.Read();
        var expected = new List<Cheep>(){_cheep1, _cheep2, _cheep3};
        
        Assert.Equal(expected, actual);

        controller.Store(_cheep4);
    }



    [Fact]
    public void Write_Cheep_To_Database()
    {
        var controller = new CSVDatabase<Cheep>();
        controller.Store(_cheepTest);

        var reader = new CSVDatabase<Cheep>();
        var actual = reader.Read();
        var expected = new List<Cheep>(){_cheep1, _cheep2, _cheep3, _cheep4, _cheepTest};

        Assert.Equal(expected, actual);
        
        controller.Delete(_cheepTest);
    }
}
