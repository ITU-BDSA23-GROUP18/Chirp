using Microsoft.Data.Sqlite;
using System.Data;
using System.Diagnostics;
namespace Chirp.Razor;

public class DBFacade {
    private string _sqlDBFilePath;
    public DBFacade(string? sqlDBFilePath) {
        if(sqlDBFilePath == null){
            sqlDBFilePath = "/tmp/cheepDatabase.db";
        }
        InitDB(sqlDBFilePath);
        _sqlDBFilePath = sqlDBFilePath;
    }
    /*
    * modified this a little 
    * https://stackoverflow.com/questions/20764049/how-do-i-execute-a-shell-script-in-c
    */
    private void InitDB(string dbPath)
    {
        string schemaPath = "data/schema.sql";
        string dumpPath = "data/dump.sql";

        ExecuteSqliteCommand(dbPath,schemaPath);
        ExecuteSqliteCommand(dbPath, dumpPath);
        
    }
    private void ExecuteSqliteCommand(string dbPath, string sqlPath){
        //insted of running a script we can just run the sqlite3 commands here such. 
        //such that if "CHIRPDBPATH=./mychirp.db dotnet run" is run the mychirp.db is still initialized 
        ProcessStartInfo startInfo = new()
        {
            FileName = "/bin/sh",
            Arguments = $"-c \"sqlite3 {dbPath} < {sqlPath}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        Process process = new Process { StartInfo = startInfo };
        try
        {
            process.Start();
            // To capture the script's errors:
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (!string.IsNullOrEmpty(errors))
            {
                Console.WriteLine($"Errors: {errors}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
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
