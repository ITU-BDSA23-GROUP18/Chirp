using Chirp.CLI;
using SimpleDB;
using DocoptNet;

const string usage = @"Chirp.

Usage:
  chirp.exe cheep <message>
  chirp.exe read
 

Options:
  -h --help     Show this screen.
  --version     Show version.

";

var arguments = new Docopt().Apply(usage, args, version: "Chirp 0.1", exit: true)!;

IDatabaseRepository<Cheep> databaseRepository = new CSVDatabase<Cheep>();

if (arguments["read"].IsTrue)
{
    // Read cheeps
    databaseRepository.Read().ToList().ForEach(cheep =>
    {
        Console.WriteLine($"{cheep.Author} @ {cheep.Timestamp}: {cheep.Message}");
    });   
}

// Post a cheep
if (arguments["cheep"].IsTrue)
{
    
    // Check for enough command line arguments
    if (arguments["<message>"].IsNullOrEmpty || arguments["<message>"].ToString() == "")
    {
        throw new Exception("What is your message?");
    }
    var cheep = new Cheep(Environment.UserName, arguments["<message>"].ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    databaseRepository.Store(cheep);
    //store the data
    
}