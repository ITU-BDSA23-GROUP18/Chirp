using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Reflection;
namespace Chirp.Razor;

public class DBFacade {
    public DBFacade() {
        using var db = new CheepContext();
        //create a author

        var author = new Author { Name = "Test Author", Email = "hans@grete.com"};
        db.Authors.Add(author);

        //create a cheep for the author
        var Cheep = new Cheep {Author = author, Text = "Hello World", TimeStamp = DateTime.UtcNow};
        db.Cheeps.Add(Cheep);
        db.SaveChanges();
        //get all cheeps and display
        var cheeps = db.Cheeps.Include(c => c.Author).ToList();
        
        foreach (var c in cheeps)
        {
            Debug.WriteLine($"{c.Author?.Name} - {c.Text} - {c.TimeStamp}");
        }
    }

    public List<CheepViewModel> GetCheeps(int skip, int count)
    {
        using var db = new CheepContext();
        var cheeps = db.Cheeps
            .Include(c => c.Author)
            .Skip(skip)
            .Take(count)
            .Select(c => new CheepViewModel(c.Author.Name,c.Text, UnixTimeStampToDateTimeString(c.TimeStamp.Second)))
            .ToList();
        return cheeps;
    }
 
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int skip, int count)
    {
        using var db = new CheepContext();
        var cheeps = db.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == author)
            .Skip(skip)
            .Take(count)
            .Select(c => new CheepViewModel(c.Author.Name,c.Text, UnixTimeStampToDateTimeString(c.TimeStamp.Second)))
            .ToList();
        return cheeps;
    }
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}
