namespace Repositories;

public interface ICheepRepository
{
    //Get cheeps
    public Task<IEnumerable<CheepDTO>> GetCheep(int page = 1);
    public Task<IEnumerable<CheepDTO>> GetCheepFromAuthor(Author author, int page = 1);
    
    //Post cheeps
    public void CreateCheep(string message, Guid currentUserId);
}
