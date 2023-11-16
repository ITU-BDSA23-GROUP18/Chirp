namespace Chirp.Infrastructure;

public class ReactionRepository : IReactionRepository
{
    private readonly ChirpContext _reactionDb;

    public ReactionRepository(ChirpContext reactionDb)
    {
        _reactionDb = reactionDb;
        _reactionDb.InitializeDatabase();
    }

    public IEnumerable<string> GetAllReactionTypes() =>
        Enum.GetValues<ReactionType>().Select(r => r.ToString());
    
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
            Cheep = cheep, 
            Author = author, 
            ReactionType = reactionType
        };

        _reactionDb.Reactions.Add(reaction);
        cheep.Reactions.Add(reaction);
        
        _reactionDb.SaveChanges();
    }

    public void RemoveReaction(string cheepId, string authorName)
    {
        var reaction = _reactionDb.Reactions.FirstOrDefault(r => 
            r.Cheep.CheepId == new Guid(cheepId) && r.Author.Name == authorName);
        if (reaction != null) _reactionDb.Reactions.Remove(reaction);
        _reactionDb.SaveChanges();
    }
}
