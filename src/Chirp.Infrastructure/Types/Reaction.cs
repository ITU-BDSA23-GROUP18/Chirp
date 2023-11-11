namespace Chirp.Infrastructure;

public class Reaction
{
    public required Cheep Cheep { get; set; }
    public required Author Author { get; set; }
    public required ReactionType ReactionType { get; set; }
}
