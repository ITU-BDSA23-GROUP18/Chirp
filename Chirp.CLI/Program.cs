using System.Text.RegularExpressions;
using Chirp.CLI;
using SimpleDB;

IDatabaseRepository<Cheep> databaseRepository = new CSVDatabase<Cheep>();

if (args[0] == "read")
{
    // Read cheeps
    databaseRepository.Read().ToList().ForEach(cheep =>
    {
        Console.WriteLine($"{cheep.Author} @ {cheep.Timestamp}: {cheep.Message}");
    });   
}

// Post a cheep
if (args[0] == "cheep")
{
    
    // Check for enough command line arguments
    if (args.Length < 2 || args[1] == "")
    {
        throw new Exception("What is your message?");
    }
    var cheep = new Cheep(Environment.UserName, args[1], DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    databaseRepository.Store(cheep);
    //store the data
    
}
