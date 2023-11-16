namespace Chirp.Infrastructure;

public static class Extensions
{
    public static DateTime ToDateTime(this int i) =>
        DateTimeOffset.FromUnixTimeSeconds(i).DateTime;

    public static DateTime ToDateTime(this string s) =>
        DateTime.Parse(s);

    public static string ShowString(this DateTime dt) =>
        dt.ToString("HH:mm:ss dd/MM/yyyy");

    public static CheepDTO ToDTO(this Cheep c) =>
        new CheepDTO(c.Author.Name, c.Message, c.TimeStamp.ShowString(), c.Reactions.Select(r => r.ToDTO()).ToList());

    public static AuthorDTO ToDTO(this Author a) =>
        new AuthorDTO(a.Name, a.Email);

    public static ReactionDTO ToDTO(this Reaction r) =>
        new ReactionDTO(r.ReactionType.ToString(), r.ToString());
}
