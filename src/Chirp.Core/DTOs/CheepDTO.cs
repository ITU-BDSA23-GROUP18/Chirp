namespace Chirp.Core.DTOs;

/// <summary>
/// The CheepDTO class is used to transfer data from the CheepRepository class to the UI
/// </summary>
public record CheepDTO
{
    public string CheepId { get; private set; }
    public string Author { get; private set; }
    public string Message { get; private set; }
    public string Timestamp { get; private set; }
    public string DisplayName { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public List<ReactionDTO> Reactions { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheepDTO"/> class.
    /// </summary>
    /// <param name="cheepId"></param>
    /// <param name="author"></param>
    /// <param name="message"></param>
    /// <param name="timestamp"></param>
    /// <param name="reactions"></param>
    /// <param name="displayName"></param>
    /// <param name="profilePictureUrl"></param>
    /// <exception cref="ArgumentNullException">Error will be thrown if any of the parameters are null or empty except for reactions.</exception>
    public CheepDTO(string cheepId, string author, string message, string timestamp, List<ReactionDTO> reactions, string displayName, string? profilePictureUrl = null)
    {
        if (author is null or "") throw new ArgumentNullException(nameof(author), "Author is null or empty");
        if (message is null or "") throw new ArgumentNullException(nameof(message), "Message is null or empty");
        if (timestamp is null or "") throw new ArgumentNullException(nameof(timestamp), "Timestamp is null or empty");
        CheepId = cheepId;
        Author = author;
        Message = message;
        Timestamp = timestamp;
        Reactions = reactions;
        DisplayName = displayName;
        ProfilePictureUrl = profilePictureUrl;
    }
}
