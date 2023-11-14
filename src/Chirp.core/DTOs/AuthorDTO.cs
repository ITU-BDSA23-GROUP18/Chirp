namespace Chirp.core;

public record AuthorDTO(string Name, string? Email){
    //the code below allows us to create a AuthorDTO and handle null values
    //the dtos are only created on the server side, so it is ware unlikely that we will get null values unless we are testing
    public AuthorDTO() : this(default!, default!) {
        if(Name is null) throw new ArgumentNullException("Name cannot be null");
        //if we in the future decide to make email mandatory, we can uncomment the line below
        //if(Email is null) throw new ArgumentNullException("Email cannot be null");
    }
}
