namespace Chirp.core;

public interface IReactionRepository
{
    // Get methods
    public void CreateReaction(int cheeepId, string authorName, string reaction);

    // Delete methods
    public void RemoveReaction(int cheeepId, string authorName);
}
