namespace Chirp.Infrastructure.Repositories;

public class ReactionRepository : IReactionRepository
{
    private readonly ChirpContext _reactionDb;
    /// <summary>
    /// Constructor for the ReactionRepository class
    /// If seedDatabase is true, the database will be seeded with data
    /// </summary>
    /// <param name="reactionDb"></param>
    public ReactionRepository(ChirpContext reactionDb)
    {
        _reactionDb = reactionDb;
        _reactionDb.InitializeDatabase(true);
    }
    
    /// <summary>
    /// Creates a reaction with the given cheepId, authorName and reactionString
    /// </summary>
    /// <param name="cheepId"></param>
    /// <param name="authorName"></param>
    /// <param name="reactionString"></param>
    /// <exception cref="ArgumentException"></exception>
    public void CreateReaction(string cheepId, string authorName, string reactionString)
    {
        var cheep = _reactionDb.Cheeps.FirstOrDefault(c => c.CheepId == new Guid(cheepId));
        var author = _reactionDb.Authors.FirstOrDefault(a => a.Name == authorName);

        if (cheep == null) throw new ArgumentException($"The given cheepId '{cheepId}' does not exist");
        if (author == null) throw new ArgumentException($"The given authorName '{authorName}' does not exist");

        var parsedReaction = Enum.TryParse<ReactionType>(reactionString, out var reactionType);
        if (!parsedReaction) throw new ArgumentException($"The given reactionType '{reactionString}' does not exist");

        var reaction = new Reaction()
        {
            CheepId = cheep.CheepId,
            Cheep = cheep,
            AuthorName = authorName,
            // Author = author,
            ReactionType = reactionType
        };

        _reactionDb.Reactions.Add(reaction);
        cheep.Reactions.Add(reaction);

        _reactionDb.SaveChanges();
    }
    
    /// <summary>
    /// Removes the reaction with the given cheepId and authorId
    /// </summary>
    /// <param name="cheepId"></param>
    /// <param name="authorName"></param>
    public void RemoveReaction(string cheepId, string authorName)
    {
        var reaction = _reactionDb.Reactions
            .Include(r => r.Cheep)
            .FirstOrDefault(r => 
                r.Cheep.CheepId == new Guid(cheepId) && 
                r.AuthorName == authorName);
        if (reaction != null)
        {
            reaction.Cheep.Reactions.Remove(reaction);
            _reactionDb.Reactions.Remove(reaction);
        }
        _reactionDb.SaveChanges();
    }
}
