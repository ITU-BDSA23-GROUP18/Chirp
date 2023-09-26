using System.Diagnostics;
using Xunit.Sdk;

namespace Chirp.CLI.Tests;

public class ProgramTests
{

    //public TestContext testContextInstance;
    
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
        hostProcess.StartInfo.Arguments = "--urls=http://localhost:5263";
        hostProcess.StartInfo.UseShellExecute = false;
        // process.StartInfo.WorkingDirectory = "";
        hostProcess.StartInfo.RedirectStandardOutput = true;
        hostProcess.Start();
        
        System.Threading.Thread.Sleep(1000);

        clientProcess.StartInfo.FileName = "../../../../../src/Chirp.CLI/bin/Debug/net7.0/win-x64/Chirp.CLI.exe";
        clientProcess.StartInfo.Arguments = "read";
        clientProcess.StartInfo.UseShellExecute = false;
        // process.StartInfo.WorkingDirectory = "";
        clientProcess.StartInfo.RedirectStandardOutput = true;
        
        System.Threading.Thread.Sleep(1000);

        clientProcess.StartInfo.RedirectStandardError = true;
        clientProcess.StartInfo.RedirectStandardOutput = true;
        
        clientProcess.Start();
        // Synchronously read the standard output of the spawned process.
        // StreamReader reader = clientProcess.StandardOutput;
        // output = reader.ReadToEnd();
        output = clientProcess.StandardOutput.ReadToEnd();
        clientProcess.WaitForExit();
        hostProcess.Kill();

        var fstCheep = output.Replace("\r", "").Split("\n")[0];
        // Assert
        //TestContext.WriteLine(output);
        Assert.Equal("", output);
        Assert.StartsWith("ropf", fstCheep);
        // Assert.EndsWith("Hello, World!", fstCheep);
    }
}
