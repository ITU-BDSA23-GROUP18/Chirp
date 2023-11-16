namespace Chirp.Infrastructure.Types;

public class Reaction
{
    public required Guid CheepId { get; set; }
    public required Guid AuthorId { get; set; }
    public required ReactionType ReactionType { get; set; }
}
