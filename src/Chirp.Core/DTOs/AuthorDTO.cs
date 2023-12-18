namespace Chirp.Core.DTOs;

/// <summary>
/// The AuthorDTO class is used to transfer data from the AuthorRepository class to the UI.
/// </summary>
public record AuthorDTO
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string DisplayName { get; private set; }
    public string? ProfilePictureUrl { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorDTO"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="email"></param>
    /// <param name="displayName"></param>
    /// <param name="profilePictureUrl"></param>
    /// <exception cref="ArgumentNullException">Error will be thrown if name is null.</exception>
    public AuthorDTO(string name, string email, string displayName, string? profilePictureUrl)
    {
        if (name == null || name.Equals("")) throw new ArgumentNullException(nameof(name), "name is null or empty");
        DisplayName = displayName;
        Name = name;
        Email = email;
        ProfilePictureUrl = profilePictureUrl;
    }
}
