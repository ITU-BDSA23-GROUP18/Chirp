namespace Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
}

public class CheepService : ICheepService
{
    private const int CheepsPerPage = 32;
    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new()
        {
            new CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel("Rasmus", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
        };

    public List<CheepViewModel> GetCheeps(int page)
    {
        page = page <= 0 ? 1 : page;
        return _cheeps.Skip(CheepsPerPage * (page - 1)).Take(CheepsPerPage).ToList();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
       page = page <= 0 ? 1 : page; 
       // filter by the provided author name
       var list = _cheeps.Where(x => x.Author == author).ToList();
       return list.Skip(CheepsPerPage * (page - 1)).Take(CheepsPerPage).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
