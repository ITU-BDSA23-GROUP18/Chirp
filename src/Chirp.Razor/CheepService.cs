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

    string sqlDBFilePath = "data/cheepDatabase.db";

    //add limit later
    public List<CheepViewModel> GetCheeps()
    {
        string sqlQuery = @"SELECT user.username, message.text, message.pub_date FROM message 
                            JOIN user on user.user_id=message.author_id
                            ORDER by message.pub_date desc";


        return getCheepsFromQuery(sqlQuery);
    }
 
    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        //prone to sql in
        string sqlQuery = 
            @$"SELECT user.username, message.text, message.pub_date 
            FROM message 
            JOIN user on user.user_id=message.author_id
            WHERE user.username = '{author}'
            ORDER by message.pub_date desc";
            
        return getCheepsFromQuery(sqlQuery);
    }
    private List<CheepViewModel> getCheepsFromQuery(string sqlQuery){
        List<CheepViewModel> result = new();

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}")) {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dataRecord = (IDataRecord)reader;
                Console.WriteLine(dataRecord.GetString(0) + " " + dataRecord.GetString(1)+ " " +dataRecord.GetInt64(2));
                //cast to CheepViewModel
                result.Add(new CheepViewModel(dataRecord.GetString(0),dataRecord.GetString(1),UnixTimeStampToDateTimeString(dataRecord.GetInt64(2))));
            }
        }
        return result;
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}
