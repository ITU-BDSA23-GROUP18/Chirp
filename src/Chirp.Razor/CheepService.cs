using Microsoft.Data.Sqlite;
using System.Data;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    //if no args create data base in tmp 

    //else if given path CHIRPDBPATH=./mychirp.db create db in current directory under the file name mychirp.db.
    static string path = Environment.GetEnvironmentVariable("CHIRPDBPATH");
    
    private readonly DBFacade db = new(path);

    //add limit later
    public List<CheepViewModel> GetCheeps()
    {
        return db.GetCheeps();
    }
 
    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {   
        return db.GetCheepsFromAuthor(author);
    }    
}
