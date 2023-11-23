namespace Chirp.Core.IRepositories;

public interface ICheepRepository
{
    // Get methods
    public Task<IEnumerable<CheepDTO>> GetCheep(int page = 1);
    public Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(string authorName, int page = 1);
    public Task<int> CountCheeps();
    public Task<int> CountCheepsFromAuthor(string authorName);

    //Post cheeps
    public void CreateCheep(string message, string username);
}
