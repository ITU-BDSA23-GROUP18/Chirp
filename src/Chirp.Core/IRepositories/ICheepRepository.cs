namespace Chirp.Core.IRepositories;

/// <summary>
/// The ICheepRepository interface is used to define the methods that the CheepRepository class must implement
/// </summary>
public interface ICheepRepository
{
    /// <summary>
    /// Gets the cheeps from the database with the given page number.
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public Task<IEnumerable<CheepDTO>> GetCheep(int page = 1);

    /// <summary>
    /// Gets the cheeps from the given author.
    /// </summary>
    /// <param name="authorName"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(string authorName, int page = 1);

    /// <summary>
    /// Counts the number of cheeps.
    /// </summary>
    /// <returns></returns>
    public Task<int> CountCheeps();

    /// <summary>
    /// Counts the number of cheeps from the given author.
    /// </summary>
    /// <param name="authorName"></param>
    /// <returns></returns>
    public Task<int> CountCheepsFromAuthor(string authorName);

    /// <summary>
    /// Creates a cheep with the given message and username
    /// </summary>
    /// <param name="message"></param>
    /// <param name="username"></param>
    public void CreateCheep(string message, string username);
}
