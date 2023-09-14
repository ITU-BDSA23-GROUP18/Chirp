namespace SimpleDB;
using CsvHelper;
using System.Globalization;
public sealed class CSVDatabase<T> : IDatabaseRepository<T>{
    public IEnumerable<T> Read(int? limit = null)
    {
        using var reader = new StreamReader("../chirp_cli_db.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<T>().ToList();
        return records;
    }
    public void Store(T record){
        using (var writer = new StreamWriter("../chirp_cli_db.csv", append: true))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }
}
