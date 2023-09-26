using System.Diagnostics;
using Xunit.Abstractions;
using Xunit.Sdk;

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
        using var hostProcess = new Process();
        using var clientProcess = new Process();
        
        hostProcess.StartInfo.FileName = "../../../../../src/Chirp.CSVDBService/bin/Debug/net7.0/Chirp.CSVDBService.exe";
        hostProcess.StartInfo.UseShellExecute = false;
        // process.StartInfo.WorkingDirectory = "";
        hostProcess.StartInfo.RedirectStandardOutput = true;
        hostProcess.Start();
        
        System.Threading.Thread.Sleep(1000);

        clientProcess.StartInfo.FileName = "../../../../../src/Chirp.CLI/bin/Debug/net7.0/Chirp.CLI.exe";
        clientProcess.StartInfo.Arguments = "read";
        clientProcess.StartInfo.UseShellExecute = false;
        // process.StartInfo.WorkingDirectory = "";
        clientProcess.StartInfo.RedirectStandardOutput = true;
        
        clientProcess.StartInfo.RedirectStandardError = true;
        clientProcess.StartInfo.RedirectStandardOutput = true;
        
        clientProcess.Start();
        // Synchronously read the standard output of the spawned process.
        // StreamReader reader = clientProcess.StandardOutput;
        // output = reader.ReadToEnd();
        output = clientProcess.StandardError.ReadToEnd();
        clientProcess.WaitForExit();
        hostProcess.Kill();
        
        var fstCheep = output.Replace("\r", "").Split("\n")[0];
        // Assert
        new TestOutputHelper().WriteLine(output);
        Assert.Equal("", output);
        Assert.StartsWith("ropf", fstCheep);
        // Assert.EndsWith("Hello, World!", fstCheep);
    }
}
