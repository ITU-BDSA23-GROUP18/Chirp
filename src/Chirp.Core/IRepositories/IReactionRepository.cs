namespace Chirp.Core.IRepositories;

/// <summary>
/// The IReactionRepository interface is used to define the methods that the ReactionRepository class must implement
/// </summary>
public interface IReactionRepository
{
    /// <summary>
    /// Gets all possible reactions and how to show them from the database
    /// </summary>
    /// <returns></returns>
    public IEnumerable<(string, string)> GetAllReactionTypes();

    /// <summary>
    /// Creates a reaction with the given cheepId, authorName and reactionType
    /// </summary>
    /// <param name="cheepId"></param>
    /// <param name="authorName"></param>
    /// <param name="reactionType"></param>
    public void CreateReaction(string cheepId, string authorName, string reactionType);

    /// <summary>
    /// Removes the reaction with the given cheepId and authorId
    /// </summary>
    /// <param name="cheepId"></param>
    /// <param name="authorName"></param>
    public void RemoveReaction(string cheepId, string authorName);
}
