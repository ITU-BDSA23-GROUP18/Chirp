namespace Chirp.Core.DTOs;

public record AuthorDTO
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string DisplayName { get; private set; }
    public AuthorDTO(string name, string email, string displayName)
    {
        if (name == null || name.Equals("")) throw new ArgumentNullException(nameof(name), "name is null or empty");
        DisplayName = displayName;
        Name = name;
        Email = email;
    }
}
