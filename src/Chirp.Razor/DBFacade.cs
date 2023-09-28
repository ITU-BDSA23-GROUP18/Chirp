using Microsoft.Data.Sqlite;
using System.Data;


public class DBFacade {
    private string _sqlDBFilePath;
    public DBFacade(string sqlDBFilePath) {
        if(sqlDBFilePath == null) sqlDBFilePath = "/tmp/cheepDatabase.db";
        _sqlDBFilePath = sqlDBFilePath;

    }
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

        using (var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}")) {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dataRecord = (IDataRecord)reader;
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
