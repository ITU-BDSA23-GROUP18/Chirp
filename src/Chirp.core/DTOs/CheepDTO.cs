using System.Buffers;

namespace Chirp.core;

public record CheepDTO{
    string Author{get;set;}
    string Message{get;set;} 
    string Timestamp{get;set;}

    public CheepDTO(string author, string message, string timestamp)
    {
        if(author == null||author.Equals("")) throw new ArgumentNullException("author is null or empty");
        if(message == null||message.Equals("")) throw new ArgumentNullException("message is null or empty");
        if(timestamp == null||timestamp.Equals("")) throw new ArgumentNullException("timestamp is null or empty");
        Author = author;
        Message = message;
        Timestamp = timestamp;
    }

}
