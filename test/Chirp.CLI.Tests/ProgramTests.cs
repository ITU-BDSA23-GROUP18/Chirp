using System.Diagnostics;

namespace Chirp.CLI.Tests;

public class ProgramTests
{
    [Fact]
    public void TestReadCheep()
    {
        // Arrange
        //ArrangeTestDatabase();
        // Act
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "../../../../../src/Chirp.CSVDBService/bin/Debug/net7.0/Chirp.CSVDBService.exe";
            process.StartInfo.Arguments = "";
            process.StartInfo.UseShellExecute = false;
            // process.StartInfo.WorkingDirectory = "";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            // Synchronously read the standard output of the spawned process.
            // StreamReader reader = process.StandardOutput;
            // output = reader.ReadToEnd();
            // process.WaitForExit();

            var line = process.StandardOutput.ReadLine();
            throw new Exception("YEAH BOI" + line);
        }
        string fstCheep = output.Split("\n")[0];
        // Assert
        Assert.StartsWith("ropf", fstCheep);
        Assert.EndsWith("Hello, World!", fstCheep);
    }
}
