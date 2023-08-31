if (args[0] == "cheep")
{
    if (args.Length < 2 || args[1] == "")
    {
        throw new Exception("What is your message?");
        return;
    }
    

    var writer = File.AppendText("./chirp_cli_db.csv");
    var message = args[1];
    var user = Environment.UserName;
    var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    writer.WriteLine($"{user},\"{message}\",{time}");
    writer.Close();
}
