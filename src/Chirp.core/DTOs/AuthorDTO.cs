namespace Chirp.core;

public record AuthorDTO{
    string Name;
    string? Email;
    public AuthorDTO(string name, string email){
        if(name == null||name.Equals("")) throw new ArgumentNullException("name is null or empty");
        Name = name;
        Email = email;
    }
}
