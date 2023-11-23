namespace Chirp.Infrastructure.Types;

public class Reaction
{
    public Guid ReactionId { get; set; }
    public required Guid CheepId { get; set; }
    public required Cheep Cheep { get; set; }
<<<<<<< HEAD
    public required string AuthorName { get; set; }
=======

    // public required Guid AuthorId { get; set; }
>>>>>>> 77392f1 (Add stylecop and fix all warnings)
    // public required Author Author { get; set; }
    public ReactionType ReactionType { get; set; }
}
