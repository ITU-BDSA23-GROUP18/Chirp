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

CSVDatabase<Cheep>.Init("../../data/chirp_cli_db.csv");
IDatabaseRepository<Cheep> databaseRepository = CSVDatabase<Cheep>.Instance;

if (arguments["read"].IsTrue)
{
    // Read cheeps
    Userinterface.PrintCheeps(databaseRepository.Read(0));
}

// Post a cheep
if (arguments["cheep"].IsTrue)
{
    // Check for enough command line arguments
    if (arguments["<message>"].IsNullOrEmpty || arguments["<message>"].ToString() == "")
    {
        throw new ArgumentException("No <message> was given");
    }
    
    var message = arguments["<message>"].ToString();
    var cheep = new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

    databaseRepository.Store(cheep);
    //store the data

}
