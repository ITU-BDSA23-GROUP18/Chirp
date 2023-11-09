namespace Chirp.core;

public interface IReactionRepository
{
    // Get methods
    public void CreateReaction(string cheepId, string authorName, string reactionType);

    // Delete methods
    public void RemoveReaction(string cheepId, string authorName);
}
