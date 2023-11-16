namespace Chirp.core;

public record AuthorDTO{
    public string Name{get; private set;}
    public string? Email{get; private set;}
    public AuthorDTO(string name, string email){
        if(name == null||name.Equals("")) throw new ArgumentNullException("name is null or empty");
        Name = name;
        Email = email;
    }
}
