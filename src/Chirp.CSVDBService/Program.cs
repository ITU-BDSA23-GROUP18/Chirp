using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/cheeps", () => CSVDatabase<Cheep>.Instance.Read(-1));

app.MapPost("/cheep", (Cheep cheep) => {new NotImplementedException();});

app.Run();

public record Cheep(string Author, string Message, long Timestamp);
