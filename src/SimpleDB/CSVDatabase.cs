namespace SimpleDB;
using CsvHelper;
using System.Globalization;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private static CSVDatabase<T>? _instance;
    private readonly string _path;

    public CSVDatabase(string path)
    {
        _path = path;
    }

    private static readonly string defaultCsvFileData = "Author,Message,Timestamp\nropf,\"Hello, BDSA students!\",1690891760\nrnie,\"Welcome to the course!\",1690978778\nrnie,\"I hope you had a good summer.\",1690979858\nropf,\"Cheeping cheeps on Chirp :)\",1690981487";
    /*
    https://csharpindepth.com/articles/singleton
    */
    public static CSVDatabase<T> Instance
    {
        get
        {
            if (_instance == null)
            {
                var defaultCSVDatabasePath = "../../data/chirp_cli_db.csv";
                if (File.Exists(defaultCSVDatabasePath)) {
                    _instance = new CSVDatabase<T>(defaultCSVDatabasePath);
                } else {
                    var file = File.CreateText("/tmp/chirp_cli_db.csv");
                    file.Write(defaultCsvFileData);
                    file.Close();
                    _instance = new CSVDatabase<T>("/tmp/chirp_cli_db.csv");
                }
            }
            return _instance;
        }
    }

    public static void Init(string path)
    {
        if (_instance == null)
        {
            _instance = new CSVDatabase<T>(path);
        }
    }
    
    public IEnumerable<T> Read(int limit)
    {
        using var reader = new StreamReader(_path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        
        while (csv.Read() && limit != 0)
        {
            limit--;
            
            var record = csv.GetRecord<T>();
            if (record != null) {
                yield return record;
            }
        }
    }
    
    public void Store(T record)
    {
        using (var writer = new StreamWriter(_path, append: true))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }
    
    public void Delete(T record)
    {
        var cheeps = Read(-1);
        var newCheeps = new List<T>();
        foreach (var cheep in cheeps)
        {
            if (!cheep!.Equals(record)) 
            { 
                newCheeps.Add(cheep); 
            }
        }
        using (var writer = new StreamWriter(Path.GetFullPath(_path), append: false))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(newCheeps);
        }
    }
}
