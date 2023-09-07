namespace Chirp.CLI;

public static class Userinterface{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps){

        cheeps.ToList().ForEach(cheep =>
        {
            Console.WriteLine($"{cheep.Author} @ {cheep.Timestamp}: {cheep.Message}");
        });
    }
    public static Cheep CreateCheep(string message){
        var cheep = new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        return cheep;
    }
}