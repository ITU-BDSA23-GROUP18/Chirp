using Microsoft.Data.Sqlite;
using System.Data;
using System.Reflection;
namespace Chirp.Razor;

public class DBFacade {
    private string _sqlDBFilePath;
    public DBFacade(string? sqlDBFilePath) {
        if(sqlDBFilePath == null){
            sqlDBFilePath = "/tmp/chirp.db";
        }
        if (!File.Exists(sqlDBFilePath)) {
            SeedNewDB(sqlDBFilePath);
        }
        _sqlDBFilePath = sqlDBFilePath;
    }

    private static void SeedNewDB(string dbPath)
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        var sqlFilesToRun = new string[] { "Chirp.Razor.data.schema.sql", "Chirp.Razor.data.dump.sql" };
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var filePath in sqlFilesToRun)
        {
            var fileStream = assembly.GetManifestResourceStream(filePath);
            if (fileStream == null)
            {
                throw new FileNotFoundException("SQL File not found: " + filePath);
            }
            var data = new StreamReader(fileStream).ReadToEnd();
            var commmand = connection.CreateCommand();
            commmand.CommandText = data;
            commmand.ExecuteNonQuery();
        }
    }
    public List<CheepViewModel> GetCheeps(int skip, int count)
    {
        string sqlQuery = @"SELECT user.username, message.text, message.pub_date FROM message 
                            JOIN user on user.user_id=message.author_id
                            ORDER by message.pub_date desc
                            LIMIT $skip, $count;";
                            
        var skipParam = new SqliteParameter("$skip", skip);
        var countParam = new SqliteParameter("$count", count);
        return GetCheepsFromQuery(sqlQuery, skipParam, countParam);
    }
 
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int skip, int count)
    {
        //prone to sql in
        string sqlQuery = 
            @"SELECT user.username, message.text, message.pub_date 
            FROM message 
            JOIN user on user.user_id=message.author_id
            WHERE user.username = $author
            ORDER by message.pub_date desc
            LIMIT $skip, $count;";

        var authorParam = new SqliteParameter("$author", author);
        var skipParam = new SqliteParameter("$skip", skip);
        var countParam = new SqliteParameter("$count", count);
        return GetCheepsFromQuery(sqlQuery, authorParam, skipParam, countParam);
    }
     private List<CheepViewModel> GetCheepsFromQuery(string sqlQuery, params SqliteParameter[] paramArray){
        List<CheepViewModel> result = new();

        using (var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}")) {
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            command.Parameters.AddRange(paramArray);

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
