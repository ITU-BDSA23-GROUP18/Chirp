namespace Chirp.core;

public record CheepDTO(string Author, string Message, string Timestamp){
    //the code below allows us to create a CheepDTO and handle null values
    //the dtos are only created on the server side, so it is ware unlikely that we will get null values unless we are testing
    public CheepDTO() : this(default!, default!, default!) {
        if(Author is null) throw new ArgumentNullException("Author cannot be null");
        if(Message is null) throw new ArgumentNullException("Message cannot be null");
        if(Timestamp is null) throw new ArgumentNullException("Timestamp cannot be null");
    }
    
    public int cheepMaxLenght = 160;
}
