if (args[0] == "cheep")
{
    var writer = new StreamWriter("./chirp_cli_db.csv", true);
    var message = args[1];
    var user = Environment.UserName;
    var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    writer.WriteLine($"{user}, \"{message}\", {time}");
    writer.Close();
}


