namespace SimpleDB;
using CsvHelper;
using System.Globalization;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private static CSVDatabase<T>? _instance;
    private readonly string _path;

    private CSVDatabase(string path)
    {
        _path = path;
    }

    /*
    https://csharpindepth.com/articles/singleton
    */
    public static CSVDatabase<T> Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CSVDatabase<T>("../../data/chirp_cli_db.csv");
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
        var records = csv.GetRecords<T>().ToList();
        return records;
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
}
