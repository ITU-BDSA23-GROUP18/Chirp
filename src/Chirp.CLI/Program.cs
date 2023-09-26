using System.Data;
using Chirp.CLI;
using DocoptNet;
using System.Net.Http.Headers;
using System.Net.Http.Json;

const string usage = @"Chirp.

Usage:
  chirp.exe cheep <message>
  chirp.exe read [<readLimit>]

Options:
  -h --help     Show this screen.
  --version     Show version.
";


var arguments = new Docopt().Apply(usage, args, version: "Chirp 0.1", exit: true)!;

// Create an HTTP client object
var baseURL = "http://localhost:5263";

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
client.BaseAddress = new Uri(baseURL);

if (arguments["read"].IsTrue)
{
    int limit = -1;
    if (!arguments["<readLimit>"].IsNullOrEmpty) limit = int.Parse(arguments["<readLimit>"].ToString());
    
    // Read cheeps
    var cheeps = await client.GetFromJsonAsync<IEnumerable<Cheep>>("cheeps");
    if (cheeps == null) throw new DataException("Nothing returned from the WEB.API");
    Userinterface.PrintCheeps(cheeps);
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
    
    // Store the data
    await client.PostAsJsonAsync("cheep", cheep);
}
