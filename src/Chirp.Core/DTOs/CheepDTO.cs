namespace Chirp.Core.DTOs;

public record CheepDTO
{
    public string CheepId { get; private set; }
    public string Author { get; private set; }
    public string Message { get; private set; }
    public string Timestamp { get; private set; }
    public List<ReactionDTO> Reactions { get; private set; }

    public CheepDTO(string cheepId, string author, string message, string timestamp, List<ReactionDTO> reactions)
    {        
        if (cheepId == null || cheepId.Equals("")) throw new ArgumentNullException(nameof(author), "CheepId is null or empty");
        if (author == null || author.Equals("")) throw new ArgumentNullException(nameof(author), "Author is null or empty");
        if (message == null || message.Equals("")) throw new ArgumentNullException(nameof(message), "Message is null or empty");
        if (timestamp == null || timestamp.Equals("")) throw new ArgumentNullException(nameof(timestamp), "Timestamp is null or empty");
        CheepId = cheepId;
        Author = author;
        Message = message;
        Timestamp = timestamp;
        Reactions = reactions;
    }
}
