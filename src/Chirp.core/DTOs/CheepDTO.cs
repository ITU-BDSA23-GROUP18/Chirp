namespace Chirp.core;

public record CheepDTO(string Author, string Message, string Timestamp){
    //the code below allows us to create a CheepDTO and handle null values
    public CheepDTO() : this(default!, default!, default!) {
        if(Author is null) throw new ArgumentNullException("Author cannot be null");
        if(Message is null) throw new ArgumentNullException("Message cannot be null");
        if(Timestamp is null) throw new ArgumentNullException("Timestamp cannot be null");
    }
    
    public int maxLenght = 160;
}
