namespace Repositories;

public static class Extensions
{
    public static DateTime ToDateTime(this int i) =>
        DateTimeOffset.FromUnixTimeSeconds(i).DateTime;

    public static string ToString(this DateTime dt) =>
        dt.ToString("hh:mm:ss dd/MM/yyyy");
}
