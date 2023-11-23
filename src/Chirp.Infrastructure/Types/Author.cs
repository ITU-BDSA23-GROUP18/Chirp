namespace Chirp.Infrastructure.Types;

public class Author
{
    public Guid AuthorId { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string Email { get; set; } = ""; //TODO: Why is this not required?
    public List<Cheep> Cheeps { get; set; } = new ();
    public List<Author> Following { get; set; } = new ();
    public List<Author> Followers { get; set; } = new ();
}
