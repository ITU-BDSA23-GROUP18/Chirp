namespace Chirp.CLI;

public static class Userinterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {

        cheeps.ToList().ForEach(cheep =>
        {
            Console.WriteLine($"{cheep.Author} @ {cheep.Timestamp}: {cheep.Message}");
        });
    }
}
