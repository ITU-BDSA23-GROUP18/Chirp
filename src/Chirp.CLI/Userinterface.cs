namespace Chirp.CLI;

public static class Userinterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        cheeps.ToList().ForEach(cheep =>
        {
            Console.WriteLine($"{cheep.Author} @ {DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).DateTime}: {cheep.Message}");
        });
    }
}
