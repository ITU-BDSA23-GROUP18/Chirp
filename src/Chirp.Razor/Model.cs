namespace Chirp.Razor;

public class Cheep
{
    public int CheepId { get; set; }
    public Author Author { get; set; }
    public string Text { get; set; }
    public DateTime TimeStamp { get; set; }

    public Cheep(Author author, string text, DateTime timeStamp)
    {
        Author = author;
        Text = text;
        TimeStamp = timeStamp;
    }
}
public class Author
{
    public int AuthorId { get; set; }
    public string Name { get; set; }

    public string Email { get; set; }
    public List<Cheep> Cheeps { get; set;} = new ();

    public Author(string name, string email)
    {
        Name = name;
        Email = email;
    }
}

//Cheep and Author.
