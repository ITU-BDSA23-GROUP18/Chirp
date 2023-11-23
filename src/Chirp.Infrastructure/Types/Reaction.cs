namespace Chirp.Infrastructure.Types;

public class Reaction
{
    public Guid ReactionId { get; set; }
    public required Guid CheepId { get; set; }
    public required Cheep Cheep { get; set; }
    public required Guid AuthorId { get; set; }
    // public required Author Author { get; set; }
    public ReactionType ReactionType { get; set; }
}
