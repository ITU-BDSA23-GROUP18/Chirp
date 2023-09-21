using Chirp.CLI;
using SimpleDB;
using DocoptNet;

const string usage = @"Chirp.

Usage:
  chirp.exe cheep <message>
  chirp.exe read [<readLimit>]

Options:
  -h --help     Show this screen.
  --version     Show version.
";

var arguments = new Docopt().Apply(usage, args, version: "Chirp 0.1", exit: true)!;

CSVDatabase<Cheep>.Init("../../data/chirp_cli_db.csv");
IDatabaseRepository<Cheep> databaseRepository = CSVDatabase<Cheep>.Instance;

if (arguments["read"].IsTrue)
{
    int limit = -1;
    if (!arguments["<readLimit>"].IsNullOrEmpty) {
        limit = int.Parse(arguments["<readLimit>"].ToString());
    } 
    // Read cheeps
    Userinterface.PrintCheeps(databaseRepository.Read(limit));
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
