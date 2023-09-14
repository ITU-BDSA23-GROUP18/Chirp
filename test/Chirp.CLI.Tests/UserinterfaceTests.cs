namespace Chirp.CLI.Tests;

public class UserinterfaceTests
{
    private readonly Cheep _cheep1 = new Cheep("ropf","Hello, BDSA students!", 1690891760);
    private readonly Cheep _cheep2 = new Cheep("rnie","Welcome to the course!",1690978778);
    private readonly Cheep _cheep3 = new Cheep("rnie","I hope you had a good summer.",1690979858);
    private readonly Cheep _cheep4 = new Cheep("ropf","Cheeping cheeps on Chirp :)",1690981487);
    private readonly string _data1 = "ropf @ 01-08-2023 12:09:20: Hello, BDSA students!";
    private readonly string _data2 = "rnie @ 02-08-2023 12:19:38: Welcome to the course!";
    private readonly string _data3 = "rnie @ 02-08-2023 12:37:38: I hope you had a good summer.";
    private readonly string _data4 = "ropf @ 02-08-2023 13:04:47: Cheeping cheeps on Chirp :)";

    [Fact]
    public void PrintCheeps_Given1Cheep_PrintsOneLineToOutput()
    {
        // Arrange
        var writer = new StringWriter();
        Console.SetOut(writer);
        
        // Act
        Userinterface.PrintCheeps(new [] {_cheep1});
        
        // Assert
        var output = writer.ToString();
        Assert.Equal($"{_data1}\r\n", output);
    }
    
    [Fact]
    public void PrintCheeps_Given5Cheeps_PrintsReadableTextToOutput()
    {
        // Arrange
        var writer = new StringWriter();
        Console.SetOut(writer);
        
        // Act
        Userinterface.PrintCheeps(new [] {_cheep1, _cheep2, _cheep3, _cheep4});
        
        // Assert
        var output = writer.ToString();
        Assert.Equal($"{_data1}\r\n{_data2}\r\n{_data3}\r\n{_data4}\r\n", output);
    }
    
    [Fact]
    public void PrintCheeps_GivenEmptyList_PrintsNothingToOutput()
    {
        // Arrange
        var writer = new StringWriter();
        Console.SetOut(writer);
        
        // Act
        Userinterface.PrintCheeps(new Cheep[0]);
        
        // Assert
        var output = writer.ToString();
        Assert.Equal("", output);
    }
    
    [Theory]
    [InlineData(1690891760, "01-08-2023 12:09:20")]
    [InlineData(1690978778, "02-08-2023 12:19:38")]
    [InlineData(1690979858, "02-08-2023 12:37:38")]
    [InlineData(1690981487, "02-08-2023 13:04:47")]
    public void FromUnixTimeSeconds_GivenSeconds_ReturnsDateTimeString(int timestamp, string expected)
    {
        // Arrange

        // Act
        var actual = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime.ToString();
        
        // Assert
        Assert.Equal(expected, actual);
    }
}
