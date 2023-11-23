namespace Chirp.Core.DTOs;

public record CheepDTO
{
    public string Author { get; private set; }
    public string Message { get; private set; }
    public string Timestamp { get; private set; }
    public List<ReactionDTO> Reactions { get; private set; }

    public CheepDTO(string author, string message, string timestamp, List<ReactionDTO> reactions)
    {
        if (author == null || author.Equals("")) throw new ArgumentNullException(nameof(author), "Author is null or empty");
        if (message == null || message.Equals("")) throw new ArgumentNullException(nameof(message), "Message is null or empty");
        if (timestamp == null || timestamp.Equals("")) throw new ArgumentNullException(nameof(timestamp), "Timestamp is null or empty");
        Author = author;
        Message = message;
        Timestamp = timestamp;
        Reactions = reactions;
    }
}
