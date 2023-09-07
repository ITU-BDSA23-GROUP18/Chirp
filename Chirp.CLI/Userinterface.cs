namespace Chirp.CLI;

static class Userinterface{
    static void PrintCheeps(IEnumerable<Cheep> cheeps){

        cheeps.Read().ToList().ForEach(cheep =>
        {
        Console.WriteLine($"{cheep.Author} @ {cheep.Timestamp}: {cheep.Message}");
        });
    }
}