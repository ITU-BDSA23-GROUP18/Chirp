namespace Chirp.Infrastructure.Types;

public class Author
{
    public Guid AuthorId { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string DisplayName { get; set; } = "";
    public string Email { get; set; } = "";
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public List<Author> Following { get; set; } = new List<Author>();
    public List<Author> Followers { get; set; } = new List<Author>();
    public string? ProfilePictureUrl { get; set; }

    // for individual user/"Author" preferences:
    public bool IsDarkMode { get; set; }
    public float FontSizeScale { get; set; }
}
